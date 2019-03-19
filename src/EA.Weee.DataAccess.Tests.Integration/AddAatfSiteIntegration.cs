namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedReused;
    using Xunit;
    using AatfAddress = Domain.AatfReturn.AatfAddress;
    using Country = Domain.Country;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;

    public class AddAatfSiteIntegration
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

                var returnData = await CreateWeeeReusedSite(context, dataAccess, aatfAddress);

                var testWeeeReusedSite = context.WeeeReusedSite
                                            .Where(t => t.WeeeReused.ReturnId == returnData.Item1
                                                    && t.WeeeReused.Aatf.Id == returnData.Item2).FirstOrDefault();

                var testAddress = testWeeeReusedSite.Address;

                testAddress.Should().NotBeNull();
                testAddress.Name.Should().NotBeNullOrEmpty();
                testAddress.Name.Should().Be("Site");
                testAddress.Address1.Should().NotBeNullOrEmpty();
                testAddress.Address1.Should().Be("Address1");
                testAddress.Address2.Should().Be("Address2");
                testAddress.TownOrCity.Should().NotBeNullOrEmpty();
                testAddress.TownOrCity.Should().Be("Town");
                testAddress.CountyOrRegion.Should().Be("County");
                testAddress.Postcode.Should().Be("PO12ST34");
                testAddress.CountryId.Should().NotBeEmpty();
                testAddress.CountryId.Should().Be(country.Id);
            }
        }

        private async Task<Tuple<Guid, Guid>> CreateWeeeReusedSite(WeeeContext context,
            AatfSiteDataAccess dataAccess, AatfAddress aatfAddress)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator);
            
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
    }
}
