namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedReused;
    using Xunit;
    using AatfAddress = Domain.AatfReturn.AatfAddress;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;

    public class AatfSiteIntegration
    {
        [Fact]
        public async Task CanCreateAatfAddressEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var aatfAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var dataAccess = new AatfSiteDataAccess(context, A.Fake<IGenericDataAccess>());

                var returnData = await CreateWeeeReusedSite(context, dataAccess, aatfAddress, database);

                AssertSubmitted(context, country, returnData, aatfAddress);
            }
        }

        [Fact]
        public async void UpdateAddress_GivenAddressToUpdate_ContextShouldContainUpdatedAddress()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var aatfAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var dataAccess = new AatfSiteDataAccess(context, A.Fake<IGenericDataAccess>());

                var returnData = await CreateWeeeReusedSite(context, dataAccess, aatfAddress, database);

                var oldAddress = (context.WeeeReusedSite
                                        .Where(t => t.WeeeReused.ReturnId == returnData.Item1
                                                && t.WeeeReused.Aatf.Id == returnData.Item2).FirstOrDefault()).Address;
                var newAddress = new SiteAddressData("Site1", "Address11", "Address21", "Town1", "County1", "PO12ST341", country.Id, country.Name);
                var newCountry = await context.Countries.SingleAsync(c => c.Name == "Germany");

                await dataAccess.Update(oldAddress, newAddress, newCountry);

                AssertUpdated(context, returnData, newAddress, newCountry);
            }
        }

        private async Task<Tuple<Guid, Guid>> CreateWeeeReusedSite(WeeeContext context,
            AatfSiteDataAccess dataAccess, AatfAddress aatfAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

            context.Organisations.Add(organisation);
            context.Operators.Add(@operator);
            context.Schemes.Add(scheme);
            context.Aatfs.Add(aatf);
            context.Returns.Add(@return);
            
            await context.SaveChangesAsync();

            var weeeReused = new WeeeReused(aatf.Id, @return.Id);
            var weeeReusedAmount = new List<WeeeReusedAmount>();

            context.WeeeReused.Add(weeeReused);
            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                context.WeeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, (int)category, (int)category, (int)category + 1));
            }

            await context.SaveChangesAsync();
            
            var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

            await dataAccess.Submit(weeeReusedSite);

            return Tuple.Create(@return.Id, aatf.Id);
        }

        private static void AssertSubmitted(WeeeContext context, Domain.Country country, Tuple<Guid, Guid> returnData, AatfAddress aatfAddress)
        {
            var testAddress = (context.WeeeReusedSite
                                        .Where(t => t.WeeeReused.ReturnId == returnData.Item1
                                                && t.WeeeReused.Aatf.Id == returnData.Item2).FirstOrDefault()).Address;

            testAddress.Should().NotBeNull();
            testAddress.Name.Should().NotBeNullOrEmpty();
            testAddress.Name.Should().Be(aatfAddress.Name);
            testAddress.Address1.Should().NotBeNullOrEmpty();
            testAddress.Address1.Should().Be(aatfAddress.Address1);
            testAddress.Address2.Should().Be(aatfAddress.Address2);
            testAddress.TownOrCity.Should().NotBeNullOrEmpty();
            testAddress.TownOrCity.Should().Be(aatfAddress.TownOrCity);
            testAddress.CountyOrRegion.Should().Be(aatfAddress.CountyOrRegion);
            testAddress.Postcode.Should().Be(aatfAddress.Postcode);
            testAddress.CountryId.Should().NotBeEmpty();
            testAddress.CountryId.Should().Be(country.Id);
        }

        private static void AssertUpdated(WeeeContext context, Tuple<Guid, Guid> returnData, SiteAddressData newAddress, Domain.Country newCountry)
        {
            var updatedAddress = (context.WeeeReusedSite
                                                    .Where(t => t.WeeeReused.ReturnId == returnData.Item1
                                                            && t.WeeeReused.Aatf.Id == returnData.Item2).FirstOrDefault()).Address;

            updatedAddress.Should().NotBeNull();
            updatedAddress.Name.Should().NotBeNullOrEmpty();
            updatedAddress.Name.Should().Be(newAddress.Name);
            updatedAddress.Address1.Should().NotBeNullOrEmpty();
            updatedAddress.Address1.Should().Be(newAddress.Address1);
            updatedAddress.Address2.Should().Be(newAddress.Address2);
            updatedAddress.TownOrCity.Should().NotBeNullOrEmpty();
            updatedAddress.TownOrCity.Should().Be(newAddress.TownOrCity);
            updatedAddress.CountyOrRegion.Should().Be(newAddress.CountyOrRegion);
            updatedAddress.Postcode.Should().Be(newAddress.Postcode);
            updatedAddress.CountryId.Should().NotBeEmpty();
            updatedAddress.CountryId.Should().Be(newCountry.Id);
        }
    }
}
