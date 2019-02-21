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
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
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
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;

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
                var addObligatedReusedDataAccess = new AddObligatedReusedDataAccess(database.WeeeContext);

                var aatfId = await addObligatedReceivedDataAccess.GetAatfId(organisation.Id);
                var schemeId = await addObligatedReceivedDataAccess.GetSchemeId(organisation.Id);

                var categoryValues = new List<ObligatedValue>();
                var weeeReceived = new WeeeReceived(schemeId, aatfId, @return.Id);
                var weeeReused = new WeeeReused(aatfId, @return.Id);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedValue((int)category, (int)category, (int)category));
                }

                var obligatedReceivedRequest = new AddObligatedReceived()
                {
                    ReturnId = @return.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var obligatedReusedRequest = new AddObligatedReused()
                {
                    ReturnId = @return.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var weeeReceivedAmount = new List<WeeeReceivedAmount>();
                var weeeReusedAmount = new List<WeeeReusedAmount>();

                foreach (var categoryValue in obligatedReceivedRequest.CategoryValues)
                {
                    weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
                }

                foreach (var categoryValue in obligatedReusedRequest.CategoryValues)
                {
                    weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
                }

                var obligateReceivedDataAccess = new AddObligatedReceivedDataAccess(database.WeeeContext);
                await obligateReceivedDataAccess.Submit(weeeReceivedAmount);

                var obligateReusedDataAccess = new AddObligatedReusedDataAccess(database.WeeeContext);
                await obligateReusedDataAccess.Submit(weeeReusedAmount);

                var fetchDataAccess = new FetchObligatedWeeeForReturnDataAccess(database.WeeeContext);

                var receivedTonnageList = await fetchDataAccess.FetchObligatedWeeeReceivedForReturn(@return.Id);
                var receivedNonHouseholdList = receivedTonnageList.Select(t => t.NonHouseholdTonnage);
                var receivedHouseholdList = receivedTonnageList.Select(t => t.HouseholdTonnage);

                var reusedTonnageList = await fetchDataAccess.FetchObligatedWeeeReceivedForReturn(@return.Id);
                var reusedNonHouseholdList = reusedTonnageList.Select(t => t.NonHouseholdTonnage);
                var reusedHouseholdList = reusedTonnageList.Select(t => t.HouseholdTonnage);

                foreach (var category in weeeReceivedAmount)
                {
                    receivedNonHouseholdList.Should().Contain(category.NonHouseholdTonnage);
                    receivedHouseholdList.Should().Contain(category.HouseholdTonnage);
                }

                foreach (var category in weeeReusedAmount)
                {
                    reusedNonHouseholdList.Should().Contain(category.NonHouseholdTonnage);
                    reusedHouseholdList.Should().Contain(category.HouseholdTonnage);
                }
            }
        }
    }
}
