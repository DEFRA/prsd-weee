namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.User;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;

    public class ReturnDataAccessTests
    {
        [Fact]
        public async Task Submit_ReturnShouldBeSubmitted()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var @return = CreateReturn(database);

                var dataAccess = new ReturnDataAccess(database.WeeeContext);

                var result = await dataAccess.Submit(@return);

                result.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task GetById_GivenValidReturnId_ReturnIsReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var @return = CreateReturn(database);

                var dataAccess = new ReturnDataAccess(database.WeeeContext);

                var id = await dataAccess.Submit(@return);

                var result = await dataAccess.GetById(id);

                result.Should().NotBeNull();
                result.Organisation.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetByOrganisationId_GivenOrganisationIdReturnsAreReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var @return1 = CreateReturn(database, organisation);
                var @return2 = CreateReturn(database, organisation);

                var dataAccess = new ReturnDataAccess(database.WeeeContext);

                await dataAccess.Submit(@return1);
                await dataAccess.Submit(@return2);

                var result = await dataAccess.GetByOrganisationId(organisation.Id);

                result.Should().NotBeNull();
                result.Count().Should().Be(2);
            }
        }

        [Fact]
        public async Task GetByComplianceYearAndYear_ReturnsWithMatchingQuarter_ReturnsShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var organisation2 = Organisation.CreateSoleTrader("Test Organisation");

                var @return1 = CreateReturn(database, organisation);
                var @return2 = CreateReturn(database, organisation);
                var @return3 = CreateReturn(database, organisation, new Quarter(2019, QuarterType.Q2));
                var @return4 = CreateReturn(database, organisation, new Quarter(2019, QuarterType.Q3));
                var @return5 = CreateReturn(database, organisation, new Quarter(2019, QuarterType.Q4));
                var @return6 = CreateReturn(database, organisation, new Quarter(2018, QuarterType.Q1));
                var @return7 = CreateReturn(database, organisation2);

                var dataAccess = new ReturnDataAccess(database.WeeeContext);

                await dataAccess.Submit(@return1);
                await dataAccess.Submit(@return2);
                await dataAccess.Submit(@return3);
                await dataAccess.Submit(@return4);
                await dataAccess.Submit(@return5);
                await dataAccess.Submit(@return6);
                await dataAccess.Submit(@return7);

                var results = await dataAccess.GetByComplianceYearAndQuarter(@return1);

                results.Count.Should().Be(2);
                results.Count(r => r.Quarter.Year != 2019).Should().Be(0);
                results.Count(r => r.Id != @return1.Id && r.Id != @return2.Id).Should().Be(0);
                results.Count(r => r.Organisation.Id != @return1.Organisation.Id).Should().Be(0);
            }
        }

        private Return CreateReturn(DatabaseWrapper database)
        {
            var organisation = Organisation.CreateSoleTrader("Test Organisation");
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Domain.AatfReturn.Return(organisation, quarter, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);
        }

        private Return CreateReturn(DatabaseWrapper database, Organisation organisation)
        { 
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Domain.AatfReturn.Return(organisation, quarter, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);
        }

        private Return CreateReturn(DatabaseWrapper database, Organisation organisation, Quarter quarter)
        {
            return new Domain.AatfReturn.Return(organisation, quarter, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);
        }
    }
}
