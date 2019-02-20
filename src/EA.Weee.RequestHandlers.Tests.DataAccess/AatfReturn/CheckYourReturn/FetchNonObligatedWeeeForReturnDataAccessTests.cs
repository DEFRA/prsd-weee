namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.CheckYourReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.CheckYourReturn;
    using Requests.AatfReturn.NonObligated;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Operator = Domain.AatfReturn.Operator;
    using Return = Domain.AatfReturn.Return;

    public class FetchNonObligatedWeeeForReturnDataAccessTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FetchNonObligatedWeeeForReturn_ReturnedListShouldContainAllTonnagesFromRequest(bool dcf)
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var context = A.Fake<WeeeContext>();

                var @return = CreateReturn();

                var returnDataAccess = new ReturnDataAccess(database.WeeeContext);

                await returnDataAccess.Submit(@return);

                var categoryValues = new List<NonObligatedValue>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new NonObligatedValue((int)category, (int)category, dcf));
                }

                var nonObligatedRequest = new AddNonObligated()
                {
                    ReturnId = @return.Id,
                    OrganisationId = @return.Operator.Organisation.Id,
                    Dcf = dcf,
                    CategoryValues = categoryValues
                };

                var nonObligatedWee = new List<NonObligatedWeee>();

                foreach (var categoryValue in nonObligatedRequest.CategoryValues)
                {
                    nonObligatedWee.Add(new NonObligatedWeee(@return, categoryValue.CategoryId, dcf, categoryValue.Tonnage));
                }

                var nonObDataAccess = new AddNonObligatedDataAccess(database.WeeeContext);

                await nonObDataAccess.Submit(nonObligatedWee);

                var dataAccess = new FetchNonObligatedWeeeForReturnDataAccess(database.WeeeContext);

                List<decimal?> tonnageList = await dataAccess.FetchNonObligatedWeeeForReturn(@return.Id, dcf);

                foreach (var category in nonObligatedWee)
                {
                    tonnageList.Should().Contain(category.Tonnage);
                }
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
