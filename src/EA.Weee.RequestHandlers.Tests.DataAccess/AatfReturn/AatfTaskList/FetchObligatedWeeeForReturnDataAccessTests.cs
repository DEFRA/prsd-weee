namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.Obligated;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class FetchObligatedWeeeForReturnDataAccessTests
    {
        private readonly FetchObligatedWeeeForReturnDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid schemeId;
        private readonly Organisation organisation;
        private readonly Operator @operator;

        public FetchObligatedWeeeForReturnDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new FetchObligatedWeeeForReturnDataAccess(context);
            schemeId = Guid.NewGuid();
            organisation = Organisation.CreateSoleTrader("Test Organisation");
            @operator = new Operator(organisation);
        }

        [Fact]
        public async Task FetchObligatedWeeeForReturn_ReturnedListShouldContainAllTonnagesFromRequest()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                var countryId = new Guid("B5EBE1D1-8349-43CD-9E87-0081EA0A8463");
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var aatf = CreateAatf(competantAuthority, @operator);

                var @return = CreateReturn(organisation);

                var returnDataAccess = new ReturnDataAccess(database.WeeeContext);

                await returnDataAccess.Submit(@return);

                var categoryValues = new List<ObligatedReceivedValue>();
                var weeeReceived = new WeeeReceived(schemeId, aatf.Id, @return.Id);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedReceivedValue((int)category, (int)category, (int)category));
                }

                var obligatedReceivedRequest = new AddObligatedReceived()
                {
                    ReturnId = @return.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var weeeReceivedAmount = new List<WeeeReceivedAmount>();

                foreach (var categoryValue in obligatedReceivedRequest.CategoryValues)
                {
                    weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
                }

                var obligatedDataAccess = new AddObligatedReceivedDataAccess(database.WeeeContext);

                await obligatedDataAccess.Submit(weeeReceivedAmount);

                var fetchDataAccess = new FetchObligatedWeeeForReturnDataAccess(database.WeeeContext);

                List<WeeeReceivedAmount> tonnageList = await dataAccess.FetchObligatedWeeeForReturn(@return.Id);

                //foreach (var category in weeeReceivedAmount)
                //{
                    //tonnageList.Should().Contain(category.NonHouseholdTonnage);
                //}
            }
        }

        private Return CreateReturn(Organisation organisation)
        {
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);

            return new Return(@operator, quarter, ReturnStatus.Created);
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, Operator @operator)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                @operator);
        }
    }
}
