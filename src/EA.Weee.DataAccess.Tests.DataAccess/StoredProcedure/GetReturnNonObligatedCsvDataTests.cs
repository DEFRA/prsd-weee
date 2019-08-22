namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Lookup;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Return = Domain.AatfReturn.Return;

    public class GetReturnNonObligatedCsvDataTests
    {
        private readonly Domain.Organisation.Organisation organisation;
        private readonly DateTime date;

        public GetReturnNonObligatedCsvDataTests()
        {
            date = new DateTime(2019, 08, 09, 11, 12, 00);

            organisation = Domain.Organisation.Organisation.CreateSoleTrader("company");
        }

        [Fact]
        public async Task Execute_GivenCreatedReturnWithNoData_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnNonObligatedCsvData(@return.Id);
                results.Count.Should().Be(14);

                var values = CategoryValues();
                for (var countValue = 0; countValue < values.Count(); countValue++)
                {
                    var value = values.ElementAt(countValue);
                    results.ElementAt(countValue).Year.Should().Be(2019);
                    results.ElementAt(countValue).Quarter.Should().Be("Q1");
                    results.ElementAt(countValue).SubmittedBy.Should().BeNullOrWhiteSpace();
                    results.ElementAt(countValue).SubmittedDate.Should().BeNull();
                    results.ElementAt(countValue).OrganisationName.Should().Be(organisation.Name);
                    results.ElementAt(countValue).Category.Should().Be($"{countValue + 1}. {value.ToDisplayString()}");
                }
            }
        }

        [Fact]
        public async Task Execute_GivenCreatedReturnWithData_ExpectedDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var values = CategoryValues();

                db.WeeeContext.Returns.Add(@return);

                foreach (var weeeCategory in values)
                {
                    db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(@return, (int)weeeCategory, false, (decimal)weeeCategory));
                    db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(@return, (int)weeeCategory, true, (decimal)weeeCategory + 1));
                }

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnNonObligatedCsvData(@return.Id);
                results.Count.Should().Be(14);

                for (var countValue = 0; countValue < values.Count(); countValue++)
                {
                    var value = values.ElementAt(countValue);
                    results.ElementAt(countValue).Year.Should().Be(2019);
                    results.ElementAt(countValue).Quarter.Should().Be("Q1");
                    results.ElementAt(countValue).SubmittedBy.Should().BeNullOrWhiteSpace();
                    results.ElementAt(countValue).SubmittedDate.Should().BeNull();
                    results.ElementAt(countValue).OrganisationName.Should().Be(organisation.Name);
                    results.ElementAt(countValue).Category.Should().Be($"{countValue + 1}. {value.ToDisplayString()}");
                    results.ElementAt(countValue).TotalNonObligatedWeeeReceived.Should().Be((decimal)value);
                    results.ElementAt(countValue).TotalNonObligatedWeeeReceivedFromDcf.Should().Be((decimal)value + 1);
                }
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturnWithData_ExpectedDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                var values = CategoryValues();

                db.WeeeContext.Returns.Add(@return);

                foreach (var weeeCategory in values)
                {
                    db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(@return, (int)weeeCategory, false, (decimal)weeeCategory));
                    db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(@return, (int)weeeCategory, true, (decimal)weeeCategory + 1));
                }

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnNonObligatedCsvData(@return.Id);
                results.Count.Should().Be(14);

                for (var countValue = 0; countValue < values.Count(); countValue++)
                {
                    var value = values.ElementAt(countValue);
                    results.ElementAt(countValue).Year.Should().Be(2019);
                    results.ElementAt(countValue).Quarter.Should().Be("Q1");
                    results.ElementAt(countValue).SubmittedBy.Should()
                        .Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                    results.ElementAt(countValue).SubmittedDate.Should().Be(date);
                    results.ElementAt(countValue).OrganisationName.Should().Be(organisation.Name);
                    results.ElementAt(countValue).Category.Should().Be($"{countValue + 1}. {value.ToDisplayString()}");
                    results.ElementAt(countValue).TotalNonObligatedWeeeReceived.Should().Be((decimal)value);
                    results.ElementAt(countValue).TotalNonObligatedWeeeReceivedFromDcf.Should().Be((decimal)value + 1);
                }
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturnWithNoData_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnNonObligatedCsvData(@return.Id);
                results.Count.Should().Be(14);

                var values = CategoryValues();
                for (var countValue = 0; countValue < values.Count(); countValue++)
                {
                    var value = values.ElementAt(countValue);
                    results.ElementAt(countValue).Year.Should().Be(2019);
                    results.ElementAt(countValue).Quarter.Should().Be("Q1");
                    results.ElementAt(countValue).SubmittedBy.Should()
                        .Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                    results.ElementAt(countValue).SubmittedDate.Should().Be(date);
                    results.ElementAt(countValue).OrganisationName.Should().Be(organisation.Name);
                    results.ElementAt(countValue).Category.Should().Be($"{countValue + 1}. {value.ToDisplayString()}");
                }
            }
        }

        private Return SetupSubmittedReturn(DatabaseWrapper db)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            SystemTime.Unfreeze();
            return @return;
        }

        private Return SetupCreatedReturn(DatabaseWrapper db)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            SystemTime.Unfreeze();
            return @return;
        }

        private IEnumerable<WeeeCategory> CategoryValues()
        {
            return Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>();
        }
    }
}
