﻿namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedReused;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;

    public class ObligatedReusedWeeeIntegration
    {
        [Fact]
        public async Task CanCreateWeeeReusedAmountEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReusedDataAccess(database.WeeeContext);

                var returnId = await CreateWeeeReusedAmounts(dataAccess, database);

                AssertValues(context, returnId);
            }
        }

        [Fact]
        public async Task UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReusedDataAccess(database.WeeeContext);

                var returnId = await CreateWeeeReusedAmounts(dataAccess, database);

                AssertValues(context, returnId);

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeReusedAmount.First(c => c.WeeeReused.ReturnId == returnId && c.CategoryId == i);
                    await dataAccess.UpdateAmounts(amount, i + 1, i + 2);
                }

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.WeeeReusedAmount.First(c => c.WeeeReused.ReturnId == returnId && c.CategoryId == i);
                    amount.HouseholdTonnage.Should().Be(i + 1);
                    amount.NonHouseholdTonnage.Should().Be(i + 2);
                }
            }
        }

        private void AssertValues(WeeeContext context, Guid returnId)
        {
            for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
            {
                var amount = context.WeeeReusedAmount.First(c => c.WeeeReused.ReturnId == returnId && c.CategoryId == i);
                amount.HouseholdTonnage.Should().Be(i);
                amount.NonHouseholdTonnage.Should().Be(i + 1);
            }
        }

        private async Task<Guid> CreateWeeeReusedAmounts(ObligatedReusedDataAccess dataAccess, DatabaseWrapper database)
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

            var weeeReused = new WeeeReused(aatf.Id, @return.Id);

            var weeeReusedAmount = new List<WeeeReusedAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, (int)category, (int)category, (int)category + 1));
            }

            await dataAccess.Submit(weeeReusedAmount);

            return @return.Id;
        }
    }
}