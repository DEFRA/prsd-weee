﻿namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Xunit;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class ObligatedReceivedWeeeIntegration
    {
        [Fact]
        public async Task CanCreateWeeeReceivedAmountEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReceivedDataAccess(database.WeeeContext);

                var returnId = await CreateWeeeReceivedAmounts(context, dataAccess, database);

                AssertValues(context, returnId);
            }
        }

        [Fact]
        public async void UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReceivedDataAccess(database.WeeeContext);

                var returnId = await CreateWeeeReceivedAmounts(context, dataAccess, database);
                
                AssertValues(context, returnId);

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeReceivedAmount.First(c => c.WeeeReceived.ReturnId == returnId && c.CategoryId == i);
                    await dataAccess.UpdateAmounts(amount, i + 1, i + 2);
                }

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeReceivedAmount.First(c => c.WeeeReceived.ReturnId == returnId && c.CategoryId == i);
                    amount.HouseholdTonnage.Should().Be(i + 1);
                    amount.NonHouseholdTonnage.Should().Be(i + 2);
                }
            }
        }

        private void AssertValues(WeeeContext context, Guid returnId)
        {
            for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
            {
                var amount = context.WeeeReceivedAmount.First(c => c.WeeeReceived.ReturnId == returnId && c.CategoryId == i);
                amount.HouseholdTonnage.Should().Be(i);
                amount.NonHouseholdTonnage.Should().Be(i + 1);
            }
        }

        private async Task<Guid> CreateWeeeReceivedAmounts(WeeeContext context,
            ObligatedReceivedDataAccess dataAccess, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            var country = await context.Countries.SingleAsync(c => c.Name == "France");
            var contact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(country);
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(context.UKCompetentAuthorities.First(), organisation, contact, country);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, database.Model.AspNetUsers.First().Id);

            context.Organisations.Add(organisation);
            context.Schemes.Add(scheme);
            context.Aatfs.Add(aatf);
            context.Returns.Add(@return);

            await context.SaveChangesAsync();

            var weeeReceived = new WeeeReceived(scheme, aatf, @return);

            var weeeReceivedAmount = new List<WeeeReceivedAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, (int)category, (int)category, (int)category + 1));
            }

            await dataAccess.Submit(weeeReceivedAmount);

            return @return.Id;
        }
    }
}