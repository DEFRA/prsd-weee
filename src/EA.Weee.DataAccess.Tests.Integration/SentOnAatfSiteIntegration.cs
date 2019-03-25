namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using AatfAddress = Domain.AatfReturn.AatfAddress;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;

    public class SentOnAatfSiteIntegration
    {
        [Fact]
        public async Task CanCreateSentOnAatfEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var aatfAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var dataAccess = new SentOnAatfSiteDataAccess(context);

                var returnData = await CreateWeeeSentOn(context, dataAccess, aatfAddress);

                var testWeeeSentOn = context.WeeeSentOn.Where(t => t.Return.Id == returnData.Item1 && t.Aatf.Id == returnData.Item2).FirstOrDefault();

                var testAddress = testWeeeSentOn.SiteAddress;

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

        [Fact]
        public async void UpdateWithOperatorAddress_GivenOperatorAddress_ContextShouldContainUpdatedWeeeSentOnWithOperatorAddress()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new SentOnAatfSiteDataAccess(database.WeeeContext);

                var country = await context.Countries.SingleAsync(c => c.Name == "France");
                var operatorCountry = await context.Countries.SingleAsync(c => c.Name == "Germany");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", operatorCountry);

                var weeeSentOn = await CreateWeeeSentOnInContext(context, dataAccess, siteAddress);

                await dataAccess.UpdateWithOperatorAddress(weeeSentOn, operatorAddress);

                weeeSentOn.OperatorAddress.Should().BeSameAs(operatorAddress);
            }
        }

        [Fact]
        public async void GetWeeeSentOnAddress_GivenWeeeSentOnId_CorrectAatfAddressShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new SentOnAatfSiteDataAccess(database.WeeeContext);

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var weeeSentOn = await CreateWeeeSentOnInContext(context, dataAccess, siteAddress);

                var result = await dataAccess.GetWeeeSentOnAddress(weeeSentOn.Id);

                result.Should().BeSameAs(weeeSentOn.SiteAddress);
            }
        }

        private async Task<Tuple<Guid, Guid>> CreateWeeeSentOn(WeeeContext context,
            SentOnAatfSiteDataAccess dataAccess, AatfAddress siteAddress)
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

            var weeeSentOn = new WeeeSentOn(siteAddress, aatf, @return);

            await dataAccess.Submit(weeeSentOn);

            return Tuple.Create(@return.Id, aatf.Id);
        }

        private async Task<WeeeSentOn> CreateWeeeSentOnInContext(WeeeContext context,
            SentOnAatfSiteDataAccess dataAccess, AatfAddress siteAddress)
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

            var weeeSentOn = new WeeeSentOn(siteAddress, aatf, @return);

            await dataAccess.Submit(weeeSentOn);

            return weeeSentOn;
        }
    }
}
