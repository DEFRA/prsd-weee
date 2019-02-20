namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Operator = Domain.AatfReturn.Operator;
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

                var @return = CreateReturn();

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

                var @return = CreateReturn();

                var dataAccess = new ReturnDataAccess(database.WeeeContext);

                var id = await dataAccess.Submit(@return);

                var result = await dataAccess.GetById(id);

                result.Should().NotBeNull();
                result.Operator.Should().NotBeNull();
                result.Operator.Organisation.Should().NotBeNull();
            }
        }

        private Return CreateReturn()
        {
            var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Domain.AatfReturn.Return(@operator, quarter, ReturnStatus.Created);
        }
    }
}
