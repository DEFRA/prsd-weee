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
    using Operator = Domain.AatfReturn.Operator;
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
                result.Operator.Should().NotBeNull();
                result.Operator.Organisation.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetByOrganisationId_GivenOrganisationIdReturnsAreReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var @operator = new Operator(organisation);
                var @return1 = CreateReturn(database, @operator);
                var @return2 = CreateReturn(database, @operator);

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
                var @operator = new Operator(organisation);
                var organisation2 = Organisation.CreateSoleTrader("Test Organisation");
                var @operator2 = new Operator(organisation2);

                var @return1 = CreateReturn(database, @operator);
                var @return2 = CreateReturn(database, @operator);
                var @return3 = CreateReturn(database, @operator, new Quarter(2019, QuarterType.Q2));
                var @return4 = CreateReturn(database, @operator, new Quarter(2019, QuarterType.Q3));
                var @return5 = CreateReturn(database, @operator, new Quarter(2019, QuarterType.Q4));
                var @return6 = CreateReturn(database, @operator, new Quarter(2018, QuarterType.Q1));
                var @return7 = CreateReturn(database, @operator2);

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
                results.Count(r => r.Operator.Id != @return1.Operator.Id).Should().Be(0);
            }
        }

        private Return CreateReturn(DatabaseWrapper database)
        {
            var organisation = Organisation.CreateSoleTrader("Test Organisation");
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Domain.AatfReturn.Return(@operator, quarter, database.Model.AspNetUsers.First().Id);
        }

        private Return CreateReturn(DatabaseWrapper database, Operator @operator)
        { 
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Domain.AatfReturn.Return(@operator, quarter, database.Model.AspNetUsers.First().Id);
        }

        private Return CreateReturn(DatabaseWrapper database, Operator @operator, Quarter quarter)
        {
            return new Domain.AatfReturn.Return(@operator, quarter, database.Model.AspNetUsers.First().Id);
        }
    }
}
