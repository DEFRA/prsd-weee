namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.Obligated;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class FetchObligatedWeeeForReturnDataAccessTests
    {
        private readonly FetchObligatedWeeeForReturnDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public FetchObligatedWeeeForReturnDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new FetchObligatedWeeeForReturnDataAccess(context);
        }

        [Fact]
        public async Task FetchObligatedWeeeForReturn_ReturnedListShouldContainAllTonnagesFromRequest()
        {
            using (var database = new DatabaseWrapper())
            {
                var companyName = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string companyRegistrationNumber = "ABC12345";

                var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);
                var scheme = new Scheme(organisation);
                var operatorTest = new Operator(organisation);
                var competentAuthority = database.WeeeContext.UKCompetentAuthorities.FirstOrDefault();
                var aatf = new Aatf(companyName, competentAuthority, companyRegistrationNumber, AatfStatus.Approved, operatorTest);
                var @return = new Return(operatorTest, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

                database.WeeeContext.Organisations.Add(organisation);
                database.WeeeContext.Schemes.Add(scheme);
                database.WeeeContext.Aatfs.Add(aatf);
                database.WeeeContext.Returns.Add(@return);
                await database.WeeeContext.SaveChangesAsync();

                var addObligatedReceivedDataAccess = new AddObligatedReceivedDataAccess(database.WeeeContext);

                var aatfId = await addObligatedReceivedDataAccess.GetAatfId(organisation.Id);
                var schemeId = await addObligatedReceivedDataAccess.GetSchemeId(organisation.Id);

                var categoryValues = new List<ObligatedValue>();
                var weeeReceived = new WeeeReceived(schemeId, aatfId, @return.Id);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedValue((int)category, (int)category, (int)category));
                }

                var obligatedReceivedRequest = new AddObligated()
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

                var tonnageList = await fetchDataAccess.FetchObligatedWeeeForReturn(@return.Id);
                var nonHouseholdList = tonnageList.Select(t => t.NonHouseholdTonnage);
                var householdList = tonnageList.Select(t => t.HouseholdTonnage);

                foreach (var category in weeeReceivedAmount)
                {
                    nonHouseholdList.Should().Contain(category.NonHouseholdTonnage);
                    householdList.Should().Contain(category.HouseholdTonnage);
                }
            }
        }
    }
}
