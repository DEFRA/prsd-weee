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
    using Weee.Tests.Core;
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
                var operatorAddress = AddressHelper.GetAatfAddress(database);
                var countrySite = await context.Countries.SingleAsync(c => c.Name == "France");
                var siteAddress = AddressHelper.GetAatfAddress(database);

                var weeeSentOn = await CreateWeeeSentOnOperatorInContext(weeeSentOnDataAccess, operatorAddress, siteAddress, database);
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

                var operatorAddress1 = AddressHelper.GetAatfAddress(database);
                var siteAddress1 = AddressHelper.GetAatfAddress(database);

                var operatorAddress2 = AddressHelper.GetAatfAddress(database);
                var siteAddress2 = AddressHelper.GetAatfAddress(database);

                var operatorAddress3 = AddressHelper.GetAatfAddress(database);
                var siteAddress3 = AddressHelper.GetAatfAddress(database);

                var weeeSentOnList = new List<WeeeSentOn>();
                weeeSentOnList.Add(await CreateWeeeSentOnContext(operatorAddress1, siteAddress1, database));
                weeeSentOnList.Add(await CreateWeeeSentOnContext(operatorAddress2, siteAddress2, database));
                weeeSentOnList.Add(await CreateWeeeSentOnContext(operatorAddress3, siteAddress3, database));

                await weeeSentOnDataAccess.Submit(weeeSentOnList);

                var weeeSentOnAmountList = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOnList[0]);
                var weeeSentOnAmountList2 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOnList[1]);
                var weeeSentOnAmountList3 = await AppendWeeeSentOnAmountToWeeeSentOn(context, weeeSentOnList[2]);

                var weeeSentOnCount = context.WeeeSentOn.Count();
                var weeeSentOnAmountCount = context.WeeeSentOnAmount.Count();
                var weeeSentOnSiteCount = context.AatfAddress.Count();

                dataAccess.Remove(weeeSentOnList[0].SiteAddress);
                dataAccess.Remove(weeeSentOnList[0].OperatorAddress);
                dataAccess.RemoveMany(weeeSentOnAmountList);
                dataAccess.Remove(weeeSentOnList[0]);

                await context.SaveChangesAsync();

                context.WeeeSentOn.Count().Should().Be(weeeSentOnCount - 1);
                context.WeeeSentOnAmount.Count().Should().Be(weeeSentOnAmountCount - 14);
                context.AatfAddress.Count().Should().Be(weeeSentOnSiteCount - 2);

                context.WeeeSentOn.Should().Contain(weeeSentOnList[1]);
                context.WeeeSentOn.Should().Contain(weeeSentOnList[2]);

                context.AatfAddress.Should().Contain(weeeSentOnList[1].SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOnList[1].OperatorAddress);
                context.AatfAddress.Should().Contain(weeeSentOnList[2].SiteAddress);
                context.AatfAddress.Should().Contain(weeeSentOnList[2].OperatorAddress);

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

        private async Task<WeeeSentOn> CreateWeeeSentOnOperatorInContext(WeeeSentOnDataAccess dataAccess, AatfAddress operatorAddress, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, database.Model.AspNetUsers.First().Id);

            database.WeeeContext.Organisations.Add(organisation);
            database.WeeeContext.Schemes.Add(scheme);
            database.WeeeContext.Aatfs.Add(aatf);
            database.WeeeContext.Returns.Add(@return);

            await database.WeeeContext.SaveChangesAsync();

            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            await dataAccess.Submit(weeeSentOn);

            return weeeSentOn;
        }

        private async Task<WeeeSentOn> CreateWeeeSentOnContext(AatfAddress operatorAddress, AatfAddress siteAddress, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, database.Model.AspNetUsers.First().Id);

            database.WeeeContext.Organisations.Add(organisation);
            database.WeeeContext.Schemes.Add(scheme);
            database.WeeeContext.Aatfs.Add(aatf);
            database.WeeeContext.Returns.Add(@return);

            await database.WeeeContext.SaveChangesAsync();

            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            return weeeSentOn;
        }

        private async Task<List<WeeeSentOnAmount>> AppendWeeeSentOnAmountToWeeeSentOn(WeeeContext context, WeeeSentOn weeeSentOn)
        {
            var weeeSentOnAmountList = new List<WeeeSentOnAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeSentOnAmountList.Add(new WeeeSentOnAmount(weeeSentOn, (int)category, (decimal?)category, (decimal?)category + 1));
            }

            context.WeeeSentOnAmount.AddRange(weeeSentOnAmountList);

            await context.SaveChangesAsync();

            return weeeSentOnAmountList;
        }
    }
}
