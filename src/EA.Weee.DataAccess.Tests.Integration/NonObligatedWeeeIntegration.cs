namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.User;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.AatfReturn.NonObligated;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;

    public class NonObligatedWeeeIntegration
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanCreateNonObligatedWeeeEntry(bool dcf)
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var name = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string crn = "ABC12345";

                var organisation = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                context.Organisations.Add(organisation);

                await context.SaveChangesAsync();

                var quarter = new Quarter(2019, QuarterType.Q1);
                var aatfReturn = new Return(organisation, quarter, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);

                var categoryValues = new List<NonObligatedValue>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new NonObligatedValue((int)category, (int)category, dcf, Guid.NewGuid()));
                }

                var nonObligatedRequest = new AddNonObligated
                {
                    ReturnId = aatfReturn.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var nonObligatedWee = new List<NonObligatedWeee>();

                foreach (var categoryValue in nonObligatedRequest.CategoryValues)
                {
                    nonObligatedWee.Add(new NonObligatedWeee(aatfReturn, categoryValue.CategoryId, categoryValue.Dcf, categoryValue.Tonnage));
                }

                var dataAccess = new NonObligatedDataAccess(database.WeeeContext);

                await dataAccess.Submit(nonObligatedWee);

                var thisTestNonObligatedWeeeArray =
                    context.NonObligatedWeee.Where(o => o.ReturnId == aatfReturn.Id).ToArray();

                Assert.NotNull(thisTestNonObligatedWeeeArray);
                Assert.NotEmpty(thisTestNonObligatedWeeeArray);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    var foundCategory = thisTestNonObligatedWeeeArray.FirstOrDefault(o => o.CategoryId == (int)category);
                    foundCategory.Should().NotBeNull();
                    var indexNum = (int)category - 1;
                    Assert.Equal(foundCategory.Dcf, nonObligatedWee[indexNum].Dcf);
                    Assert.Equal(foundCategory.Tonnage, nonObligatedWee[indexNum].Tonnage);
                    Assert.Equal(foundCategory.ReturnId, nonObligatedWee[indexNum].ReturnId);
                }
            }
        }

        [Fact]
        public async void UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new NonObligatedDataAccess(database.WeeeContext);

                var companyName = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string companyRegistrationNumber = "ABC12345";

                var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);
                var @return = new Return(organisation, new Quarter(2019, QuarterType.Q1), database.Model.AspNetUsers.First().Id, FacilityType.Aatf);

                context.Organisations.Add(organisation);
                context.Returns.Add(@return);

                await context.SaveChangesAsync();

                var nonObligatedWee = new List<NonObligatedWeee>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    nonObligatedWee.Add(new NonObligatedWeee(@return, (int)category, false, (int)category));
                }

                await dataAccess.Submit(nonObligatedWee);

                AssertValues(context, @return.Id);

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.NonObligatedWeee.First(c => c.ReturnId == @return.Id && c.CategoryId == i);
                    await dataAccess.UpdateAmount(amount, i + 1);
                }

                for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
                {
                    var amount = context.NonObligatedWeee.First(c => c.ReturnId == @return.Id && c.CategoryId == i);
                    amount.Tonnage.Should().Be(i + 1);
                }
            }
        }

        private void AssertValues(WeeeContext context, Guid returnId)
        {
            for (var i = 1; i <= Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().Count(); i++)
            {
                var amount = context.NonObligatedWeee.First(c => c.ReturnId == returnId && c.CategoryId == i);
                amount.Tonnage.Should().Be(i);
            }
        }
    }
}
