﻿namespace EA.Weee.DataAccess.Tests.Integration
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
    using Country = Domain.Country;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class ObligatedSentOnWeeeIntegration
    {
        [Fact]
        public async Task CanCreateWeeeSentOnAmountEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedSentOnDataAccess(database.WeeeContext);

                var weeeSentOnId = await CreateWeeeSentOnAmounts(dataAccess, database);

                AssertValues(context, weeeSentOnId);
            }
        }

        [Fact]
        public async void UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedSentOnDataAccess(database.WeeeContext);

                var weeeSentOnId = await CreateWeeeSentOnAmounts(dataAccess, database);

                AssertValues(context, weeeSentOnId);

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeSentOnAmount.First(c => c.WeeeSentOn.Id == weeeSentOnId && c.CategoryId == i);
                    await dataAccess.UpdateAmounts(amount, i + 1, i + 2);
                }

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeSentOnAmount.First(c => c.WeeeSentOn.Id == weeeSentOnId && c.CategoryId == i);
                    amount.HouseholdTonnage.Should().Be(i + 1);
                    amount.NonHouseholdTonnage.Should().Be(i + 2);
                }
            }
        }

        private void AssertValues(WeeeContext context, Guid weeeSentOnId)
        {
            for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
            {
                var amount = context.WeeeSentOnAmount.First(c => c.WeeeSentOn.Id == weeeSentOnId && c.CategoryId == i);
                amount.HouseholdTonnage.Should().Be(i);
                amount.NonHouseholdTonnage.Should().Be(i + 1);
            }
        }

        private async Task<Guid> CreateWeeeSentOnAmounts(ObligatedSentOnDataAccess dataAccess, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
            var contact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(country);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation, contact, country);
            var siteAddress = ObligatedWeeeIntegrationCommon.CreateAatfAddress(database);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, database.Model.AspNetUsers.First().Id);

            database.WeeeContext.Organisations.Add(organisation);
            database.WeeeContext.Schemes.Add(scheme);
            database.WeeeContext.Aatfs.Add(aatf);
            database.WeeeContext.Returns.Add(@return);
            database.WeeeContext.AatfAddress.Add(siteAddress);

            await database.WeeeContext.SaveChangesAsync();

            var weeeSentOn = new WeeeSentOn(siteAddress.Id, aatf.Id, @return.Id);

            var weeeSentOnAmount = new List<WeeeSentOnAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeSentOnAmount.Add(new WeeeSentOnAmount(weeeSentOn, (int)category, (decimal?)category, (decimal?)category + 1));
            }

           await dataAccess.Submit(weeeSentOnAmount);

           return weeeSentOn.Id;
        }
    }
}
