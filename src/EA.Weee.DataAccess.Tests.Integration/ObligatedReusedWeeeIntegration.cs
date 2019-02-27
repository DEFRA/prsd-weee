namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedReused;
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

                var returnId = await CreateWeeeReusedAmounts(context, dataAccess);

                AssertValues(context, returnId);
            }
        }

        [Fact]
        public async void UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReusedDataAccess(database.WeeeContext);

                var returnId = await CreateWeeeReusedAmounts(context, dataAccess);

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

        private async Task<Guid> CreateWeeeReusedAmounts(WeeeContext context,
            ObligatedReusedDataAccess dataAccess)
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

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, (int)category, (int)category, (int)category + 1));
            }

            await dataAccess.Submit(weeeReusedAmount);

            return @return.Id;
        }
    }
}