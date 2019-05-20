namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.Core.DataReturns;
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
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

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

                var dataAccess = new WeeeSentOnDataAccess(context);

                var returnData = await CreateWeeeSentOn(context, dataAccess, aatfAddress, database);

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
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var country = await context.Countries.SingleAsync(c => c.Name == "France");
                var operatorCountry = await context.Countries.SingleAsync(c => c.Name == "Germany");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", operatorCountry);

                var weeeSentOn = await CreateWeeeSentOnInContext(context, dataAccess, siteAddress, database);

                await dataAccess.UpdateWithOperatorAddress(weeeSentOn, operatorAddress);

                weeeSentOn.OperatorAddress.Should().BeSameAs(operatorAddress);
            }
        }

        [Fact]
        public async void GetWeeeSentOnSiteAddress_GivenWeeeSentOnId_CorrectAatfAddressShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var weeeSentOn = await CreateWeeeSentOnInContext(context, dataAccess, siteAddress, database);

                var result = await dataAccess.GetWeeeSentOnSiteAddress(weeeSentOn.Id);

                result.Should().BeSameAs(weeeSentOn.SiteAddress);
            }
        }

        [Fact]
        public async void GetWeeeSentOnOperatorAddress_GivenWeeeSentOnId_CorrectAatfAddressShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var countryOperator = await context.Countries.SingleAsync(c => c.Name == "France");

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);

                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress, siteAddress, database);

                var result = await dataAccess.GetWeeeSentOnOperatorAddress(weeeSentOn.Id);

                result.Should().BeSameAs(weeeSentOn.OperatorAddress);
            }
        }

        private async Task<Tuple<Guid, Guid>> CreateWeeeSentOn(WeeeContext context,
            WeeeSentOnDataAccess dataAccess, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await context.Countries.SingleAsync(c => c.Name == "France");
            var contact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(country);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator, contact, country);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

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
            WeeeSentOnDataAccess dataAccess, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await context.Countries.SingleAsync(c => c.Name == "France");
            var contact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(country);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator, contact, country);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

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

        private async Task<WeeeSentOn> CreateWeeeSentOnOperatorInContext(WeeeContext context,
            WeeeSentOnDataAccess dataAccess, AatfAddress operatorAddress, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await context.Countries.SingleAsync(c => c.Name == "France");
            var contact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(country);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator, contact, country);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

            context.Organisations.Add(organisation);
            context.Operators.Add(@operator);
            context.Schemes.Add(scheme);
            context.Aatfs.Add(aatf);
            context.Returns.Add(@return);

            await context.SaveChangesAsync();

            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            await dataAccess.Submit(weeeSentOn);

            return weeeSentOn;
        }

        private async Task<List<WeeeSentOnAmount>> AppendWeeeSentOnAmountToWeeeSentOn(WeeeContext context, WeeeSentOn weeeSentOn)
        {
            var weeeSentOnAmountList = new List<WeeeSentOnAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeSentOnAmountList.Add(new WeeeSentOnAmount(weeeSentOn, (int)category, (decimal?)category, (decimal?)category + 1, weeeSentOn.Id));
            }

            context.WeeeSentOnAmount.AddRange(weeeSentOnAmountList);

            await context.SaveChangesAsync();

            return weeeSentOnAmountList;
        }
    }
}
