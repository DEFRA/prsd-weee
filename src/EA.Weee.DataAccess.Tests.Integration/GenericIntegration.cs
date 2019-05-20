namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class GenericIntegration
    {
        [Fact]
        public async void RemoveWeeeSentOn_GivenWeeeSentOn_CorrectDatabaseEntriesAreDeleted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var weeeSentOnDataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var countryOperator = await context.Countries.SingleAsync(c => c.Name == "France");
                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");
                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, weeeSentOnDataAccess, operatorAddress, siteAddress, database);
                var weeeSentOnAmountList = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn);

                context.AatfAddress.Should().Contain(weeeSentOn.SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOn.OperatorAddress);
                foreach (var amount in weeeSentOnAmountList)
                {
                    context.WeeeSentOnAmount.Should().Contain(amount);
                }
                context.WeeeSentOn.Should().Contain(weeeSentOn);

                dataAccess.Remove(weeeSentOn.SiteAddress);
                dataAccess.Remove(weeeSentOn.OperatorAddress);
                dataAccess.RemoveMany(weeeSentOnAmountList);
                dataAccess.Remove(weeeSentOn);

                await context.SaveChangesAsync();

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
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var weeeSentOnDataAccess = new WeeeSentOnDataAccess(database.WeeeContext);

                var countryOperator = await context.Countries.SingleAsync(c => c.Name == "France");
                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");

                var operatorAddress = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var operatorAddress2 = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress2 = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var operatorAddress3 = new AatfAddress("Operator", "OpAddress1", "OpAddress2", "OpTown", "OpCounty", "PO12ST56", countryOperator);
                var siteAddress3 = new AatfAddress("Site", "Address1", "Address2", "Town", "County", "PO12ST34", countrySite);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(context, weeeSentOnDataAccess, operatorAddress, siteAddress, database);
                var weeeSentOn2 = await CreateWeeeSentOnOperatorInContext(context, weeeSentOnDataAccess, operatorAddress2, siteAddress2, database);
                var weeeSentOn3 = await CreateWeeeSentOnOperatorInContext(context, weeeSentOnDataAccess, operatorAddress3, siteAddress3, database);

                var weeeSentOnAmountList = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn);
                var weeeSentOnAmountList2 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn2);
                var weeeSentOnAmountList3 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOn3);

                var weeeSentOnCount = context.WeeeSentOn.Count();
                var weeeSentOnAmountCount = context.WeeeSentOnAmount.Count();
                var weeeSentOnSiteCount = context.AatfAddress.Count();

                dataAccess.Remove(weeeSentOn.SiteAddress);
                dataAccess.Remove(weeeSentOn.OperatorAddress);
                dataAccess.RemoveMany(weeeSentOnAmountList);
                dataAccess.Remove(weeeSentOn);

                await context.SaveChangesAsync();

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

        private async Task<WeeeSentOn> CreateWeeeSentOnOperatorInContext(WeeeContext context,
            WeeeSentOnDataAccess dataAccess, AatfAddress operatorAddress, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "UK - England");
            var aatfContact = new AatfContact("first", "last", "position", "address1", "address2", "town", "county", "postcode", country, "telephone", "email");
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), @operator, aatfContact, country);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

            context.AatfContacts.Add(aatfContact);
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
