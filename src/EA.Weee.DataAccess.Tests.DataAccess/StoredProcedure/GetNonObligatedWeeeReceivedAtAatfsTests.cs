namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfReturn;
    using Domain.DataReturns;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using FacilityType = Domain.AatfReturn.FacilityType;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;

    public class GetNonObligatedWeeeReceivedAtAatfsTests
    {
        private readonly Fixture fixture;

        public GetNonObligatedWeeeReceivedAtAatfsTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task Execute_GivenNonObligatedData_NonObligatedDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return1 = CreateSubmittedReturn(db);

                var @return2 = CreateSubmittedReturn(db);

                var nonObligated = new List<NonObligatedWeee>();

                foreach (var nonObligatedCategoryValue in CategoryValues())
                {
                    nonObligated.Add(new NonObligatedWeee(@return1, nonObligatedCategoryValue.CategoryId, false, nonObligatedCategoryValue.CategoryId));
                    nonObligated.Add(new NonObligatedWeee(@return2, nonObligatedCategoryValue.CategoryId, true, nonObligatedCategoryValue.CategoryId));
                }

                db.WeeeContext.NonObligatedWeee.AddRange(nonObligated);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                foreach (var nonObligatedCategoryValue in CategoryValues())
                {
                    results.First(r => r.ReturnId.Equals(@return1.Id) && r.CategoryId.Equals(nonObligatedCategoryValue.CategoryId))
                        .TotalNonObligatedWeeeReceived.Should().Be(nonObligatedCategoryValue.CategoryId);
                    results.First(r => r.ReturnId.Equals(@return2.Id) && r.CategoryId.Equals(nonObligatedCategoryValue.CategoryId))
                        .TotalNonObligatedWeeeReceivedFromDcf.Should().Be(nonObligatedCategoryValue.CategoryId);
                }
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturns_HighLevelDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var org1 = Organisation.CreateRegisteredCompany(fixture.Create<string>(), "1234567", null);
                var org2 = Organisation.CreateRegisteredCompany(fixture.Create<string>(), "1234567", null);
                var @return1 = CreateSubmittedReturn(db, org1);
                var @return2 = CreateSubmittedReturn(db, org2);

                db.WeeeContext.Returns.Add(@return1);
                db.WeeeContext.Returns.Add(@return2);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                var return1Results = results.Where(r => r.ReturnId.Equals(@return1.Id));
                foreach (var nonObligatedWeeeReceivedAtAatfsData in return1Results)
                {
                    nonObligatedWeeeReceivedAtAatfsData.Year.Should().Be(2019);
                    nonObligatedWeeeReceivedAtAatfsData.Quarter.Should().Be("Q1");
                    nonObligatedWeeeReceivedAtAatfsData.SubmittedBy.Should().Be(db.Model.AspNetUsers.First().FirstName + " " + db.Model.AspNetUsers.First().Surname);
                    nonObligatedWeeeReceivedAtAatfsData.SubmittedDate.Date.Should().Be(@return1.SubmittedDate.Value.Date);
                    nonObligatedWeeeReceivedAtAatfsData.OrganisationName.Should().Be(org1.OrganisationName);
                }
                
                var return2Results = results.Where(r => r.ReturnId.Equals(@return2.Id));
                foreach (var nonObligatedWeeeReceivedAtAatfsData in return2Results)
                {
                    nonObligatedWeeeReceivedAtAatfsData.Year.Should().Be(2019);
                    nonObligatedWeeeReceivedAtAatfsData.Quarter.Should().Be("Q1");
                    nonObligatedWeeeReceivedAtAatfsData.SubmittedBy.Should().Be(db.Model.AspNetUsers.First().FirstName + " " + db.Model.AspNetUsers.First().Surname);
                    nonObligatedWeeeReceivedAtAatfsData.SubmittedDate.Date.Should().Be(@return2.SubmittedDate.Value.Date);
                    nonObligatedWeeeReceivedAtAatfsData.OrganisationName.Should().Be(org2.OrganisationName);
                }
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturns_CategoryNamesShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return1 = CreateSubmittedReturn(db);

                db.WeeeContext.Returns.Add(@return1);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                foreach (var nonObligatedCategoryValue in CategoryValues())
                {
                    var categoryValue = results.First(r => r.CategoryId.Equals(nonObligatedCategoryValue.CategoryId));

                    categoryValue.Category.Should().Be($"{nonObligatedCategoryValue.CategoryId}. {nonObligatedCategoryValue.CategoryDisplay}");
                }
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedOrganisationWithMultipleQuarters_MultipleQuartersForOrganisationShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return1 = CreateSubmittedReturn(db);
                var @return2 = CreateSubmittedReturn(db, QuarterType.Q2, @return1.Organisation);

                db.WeeeContext.Returns.Add(@return1);
                db.WeeeContext.Returns.Add(@return2);

                var nonObligated = new List<NonObligatedWeee>();
                foreach (var nonObligatedCategoryValue in CategoryValues())
                {
                    nonObligated.Add(new NonObligatedWeee(@return1, nonObligatedCategoryValue.CategoryId, false, nonObligatedCategoryValue.CategoryId));
                    nonObligated.Add(new NonObligatedWeee(@return2, nonObligatedCategoryValue.CategoryId, true, nonObligatedCategoryValue.CategoryId));
                }

                db.WeeeContext.NonObligatedWeee.AddRange(nonObligated);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                results.Count(r => r.ReturnId.Equals(@return1.Id)).Should().Be(CategoryValues().Count);
                results.Count(r => r.ReturnId.Equals(@return2.Id)).Should().Be(CategoryValues().Count);

                foreach (var nonObligatedCategoryValue in CategoryValues())
                {
                    results.First(r => r.ReturnId.Equals(@return1.Id) && r.CategoryId.Equals(nonObligatedCategoryValue.CategoryId))
                        .TotalNonObligatedWeeeReceived.Should().Be(nonObligatedCategoryValue.CategoryId);
                    results.First(r => r.ReturnId.Equals(@return2.Id) && r.CategoryId.Equals(nonObligatedCategoryValue.CategoryId))
                        .TotalNonObligatedWeeeReceivedFromDcf.Should().Be(nonObligatedCategoryValue.CategoryId);
                }
            }
        }

        [Fact]
        public async Task Execute_GivenMultipleReturnsForOrganisationInSameQuarterAndYear_OnlyMostRecentDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Organisation.CreateRegisteredCompany(fixture.Create<string>(), "1234567", null);
                var @return1 = CreateSubmittedReturn(db, organisation);

                SystemTime.Freeze(DateTime.Now.AddDays(1));
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
                @return2.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
                SystemTime.Unfreeze();
                
                db.WeeeContext.Returns.Add(@return1);
                db.WeeeContext.Returns.Add(@return2);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                results.Count(r => r.ReturnId.Equals(@return1.Id)).Should().Be(0);
                results.Count(r => r.ReturnId.Equals(@return2.Id)).Should().Be(CategoryValues().Count);
            }
        }

        [Fact]
        public async Task Execute_GivenOrganisationNameParameter_OnlyReturnsMatchingOrganisationShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Organisation.CreateRegisteredCompany(fixture.Create<string>(), "1234567", null);
                var @return1 = CreateSubmittedReturn(db, organisation);

                db.WeeeContext.Returns.Add(@return1);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, organisation.OrganisationName);

                results.Count(r => r.OrganisationName.Equals(organisation.OrganisationName)).Should().Be(CategoryValues().Count);
            }
        }

        [Fact]
        public async Task Execute_GivenOrganisationNameParameter_OnlyReturnsPartiallyMatchingOrganisationShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Organisation.CreateRegisteredCompany(fixture.Create<string>(), "1234567", null);
                var @return1 = CreateSubmittedReturn(db, organisation);

                db.WeeeContext.Returns.Add(@return1);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, organisation.OrganisationName.Substring(0, 5));

                results.Count(r => r.OrganisationName.Equals(organisation.OrganisationName)).Should().Be(CategoryValues().Count);
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturnWithNullNonObligated_NullResultsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(2019, null);

                results.Count(r => r.ReturnId.Equals(@return.Id)).Should().Be(CategoryValues().Count);
                foreach (var nonObligatedWeeeReceivedAtAatfsData in results.Where(r => r.ReturnId.Equals(@return.Id)))
                {
                    nonObligatedWeeeReceivedAtAatfsData.TotalNonObligatedWeeeReceived.Should().BeNull();
                    nonObligatedWeeeReceivedAtAatfsData.TotalNonObligatedWeeeReceivedFromDcf.Should().BeNull();
                }
            }
        }

        private CategoryValues<NonObligatedCategoryValue> CategoryValues()
        {
            return new CategoryValues<EA.Weee.Core.AatfReturn.NonObligatedCategoryValue>();
        }

        private Return CreateSubmittedReturn(DatabaseWrapper db)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }

        private Return CreateSubmittedReturn(DatabaseWrapper db, QuarterType quarter, Organisation organisation)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019, quarter);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }

        private Return CreateSubmittedReturn(DatabaseWrapper db, EA.Weee.Domain.Organisation.Organisation organisation)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }
    }
}
