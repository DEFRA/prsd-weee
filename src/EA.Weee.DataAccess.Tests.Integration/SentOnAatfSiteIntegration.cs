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
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

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
        public async void GetWeeeSentOnSiteAddress_GivenWeeeSentOnId_CorrectAatfAddressShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", country);

                var weeeSentOn = await CreateWeeeSentOnInContext(context, dataAccess, siteAddress);

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

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress, siteAddress);

                var result = await dataAccess.GetWeeeSentOnOperatorAddress(weeeSentOn.Id);

                result.Should().BeSameAs(weeeSentOn.OperatorAddress);
            }
        }

        [Fact]
        public async void RemoveWeeeSentOn_GivenWeeeSentOn_CorrectDatabaseEntriesAreDeleted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var countryOperator = await context.Countries.SingleAsync(c => c.Name == "France");

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);

                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");

                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress, siteAddress);

                var weeeSentOnAmountList = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn);

                context.AatfAddress.Should().Contain(weeeSentOn.SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOn.OperatorAddress);
                foreach (var amount in weeeSentOnAmountList)
                {
                    context.WeeeSentOnAmount.Should().Contain(amount);
                }
                context.WeeeSentOn.Should().Contain(weeeSentOn);

                await dataAccess.RemoveWeeeSentOn(weeeSentOn, weeeSentOnAmountList);

                context.AatfAddress.Should().NotContain(weeeSentOn.SiteAddress);
                context.AatfAddress.Should().NotContain(weeeSentOn.OperatorAddress);
                foreach (var amount in weeeSentOnAmountList)
                {
                    context.WeeeSentOnAmount.Should().NotContain(amount);
                }
                context.WeeeSentOn.Should().NotContain(weeeSentOn);
            }
        }

        [Fact]
        public async void RemoveWeeeSentOn_GivenMultipleWeeeSentOn_CorrectDatabaseEntriesAreDeleted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var countryOperator = await context.Countries.SingleAsync(c => c.Name == "France");
                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var operatorAddress2 = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress2 = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var operatorAddress3 = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress3 = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress, siteAddress);
                var weeeSentOn2 = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress2, siteAddress2);
                var weeeSentOn3 = await CreateWeeeSentOnOperatorInContext(context, dataAccess, operatorAddress3, siteAddress3);

                var weeeSentOnAmountList = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn);
                var weeeSentOnAmountList2 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn2);
                var weeeSentOnAmountList3 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn3);

                var weeeSentOnCount = context.WeeeSentOn.Count();
                var weeeSentOnAmountCount = context.WeeeSentOnAmount.Count();
                var weeeSentOnSiteCount = context.AatfAddress.Count();

                await dataAccess.RemoveWeeeSentOn(weeeSentOn, weeeSentOnAmountList);

                context.WeeeSentOn.Count().Should().Be(weeeSentOnCount - 1);
                context.WeeeSentOnAmount.Count().Should().Be(weeeSentOnAmountCount - 14);
                context.AatfAddress.Count().Should().Be(weeeSentOnSiteCount - 2);

                context.WeeeSentOn.Should().Contain(weeeSentOn2);
                context.WeeeSentOn.Should().Contain(weeeSentOn3);

                context.AatfAddress.Should().Contain(weeeSentOn2.SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOn2.OperatorAddress);
                context.AatfAddress.Should().Contain(weeeSentOn3.SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOn3.OperatorAddress);

                foreach (var amount in weeeSentOnAmountList2)
                {
                    context.WeeeSentOnAmount.Should().Contain(amount);
                }

                foreach (var amount in weeeSentOnAmountList3)
                {
                    context.WeeeSentOnAmount.Should().Contain(amount);
                }
            }
        }

        private async Task<Tuple<Guid, Guid>> CreateWeeeSentOn(WeeeContext context,
            WeeeSentOnDataAccess dataAccess, AatfAddress siteAddress)
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
            WeeeSentOnDataAccess dataAccess, AatfAddress siteAddress)
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

        private async Task<WeeeSentOn> CreateWeeeSentOnOperatorInContext(WeeeContext context,
            WeeeSentOnDataAccess dataAccess, AatfAddress operatorAddress, AatfAddress siteAddress)
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
