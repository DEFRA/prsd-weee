namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.User;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;

    public class FetchObligatedReceivedWeeeForReturnDataAccessTests
    {
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
                var competentAuthority = database.WeeeContext.UKCompetentAuthorities.FirstOrDefault();
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
                var aatf = new Aatf(companyName, competentAuthority, companyRegistrationNumber, AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);
                var @return = new Return(organisation, new Quarter(2019, QuarterType.Q1), database.Model.AspNetUsers.First().Id, FacilityType.Aatf);

                database.WeeeContext.Organisations.Add(organisation);
                database.WeeeContext.Schemes.Add(scheme);
                database.WeeeContext.Aatfs.Add(aatf);
                database.WeeeContext.Returns.Add(@return);
                await database.WeeeContext.SaveChangesAsync();

                var addObligatedReceivedDataAccess = new ObligatedReceivedDataAccess(database.WeeeContext);

                var categoryValues = new List<ObligatedValue>();
                var weeeReceived = new WeeeReceived(scheme.Id, aatf.Id, @return.Id);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedValue(Guid.NewGuid(), (int)category, (int)category, (int)category));
                }

                var obligatedReceivedRequest = new AddObligatedReceived()
                {
                    ReturnId = @return.Id,
                    AatfId = aatf.Id,
                    SchemeId = scheme.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var obligatedReusedRequest = new AddObligatedReused()
                {
                    AatfId = aatf.Id,
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
                
                var obligateReceivedDataAccess = new ObligatedReceivedDataAccess(database.WeeeContext);
                await obligateReceivedDataAccess.Submit(weeeReceivedAmount);

                var fetchDataAccess = new FetchObligatedWeeeForReturnDataAccess(database.WeeeContext);

                var receivedTonnageList = await fetchDataAccess.FetchObligatedWeeeReceivedForReturn(@return.Id);
                var receivedNonHouseholdList = receivedTonnageList.Select(t => t.NonHouseholdTonnage);
                var receivedHouseholdList = receivedTonnageList.Select(t => t.HouseholdTonnage);

                foreach (var category in weeeReceivedAmount)
                {
                    receivedNonHouseholdList.Should().Contain(category.NonHouseholdTonnage);
                    receivedHouseholdList.Should().Contain(category.HouseholdTonnage);
                }
            }
        }
    }
}
