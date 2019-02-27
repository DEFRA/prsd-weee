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
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;

    public class FetchObligatedReusedWeeeForReturnDataAccessTests
    {
        private readonly FetchObligatedWeeeForReturnDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public FetchObligatedReusedWeeeForReturnDataAccessTests()
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
                var operatorTest = new Operator(organisation);
                var competentAuthority = database.WeeeContext.UKCompetentAuthorities.FirstOrDefault();
                var aatf = new Aatf(companyName, competentAuthority, companyRegistrationNumber, AatfStatus.Approved, operatorTest);
                var @return = new Return(operatorTest, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

                database.WeeeContext.Organisations.Add(organisation);
                database.WeeeContext.Aatfs.Add(aatf);
                database.WeeeContext.Returns.Add(@return);
                await database.WeeeContext.SaveChangesAsync();
                
                var addObligatedReusedDataAccess = new ObligatedReusedDataAccess(database.WeeeContext);

                var categoryValues = new List<ObligatedValue>();
                var weeeReused = new WeeeReused(aatf.Id, @return.Id);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedValue(Guid.NewGuid(), (int)category, (int)category, (int)category));
                }

                var obligatedReusedRequest = new AddObligatedReused()
                {
                    AatfId = aatf.Id,
                    ReturnId = @return.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var weeeReusedAmount = new List<WeeeReusedAmount>();
                
                foreach (var categoryValue in obligatedReusedRequest.CategoryValues)
                {
                    weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
                }
                
                var obligateReusedDataAccess = new ObligatedReusedDataAccess(database.WeeeContext);
                await obligateReusedDataAccess.Submit(weeeReusedAmount);

                var fetchDataAccess = new FetchObligatedWeeeForReturnDataAccess(database.WeeeContext);

                var reusedTonnageList = await fetchDataAccess.FetchObligatedWeeeReusedForReturn(@return.Id);
                var reusedNonHouseholdList = reusedTonnageList.Select(t => t.NonHouseholdTonnage);
                var reusedHouseholdList = reusedTonnageList.Select(t => t.HouseholdTonnage);

                foreach (var category in weeeReusedAmount)
                {
                    reusedNonHouseholdList.Should().Contain(category.NonHouseholdTonnage);
                    reusedHouseholdList.Should().Contain(category.HouseholdTonnage);
                }
            }
        }
    }
}
