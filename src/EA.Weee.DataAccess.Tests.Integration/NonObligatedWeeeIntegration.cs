namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.AatfReturn.NonObligated;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Operator = Domain.AatfReturn.Operator;
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

                var operatorTest = new Operator(organisation);
                var quarter = new Quarter(2019, QuarterType.Q1);
                var aatfReturn = new Return(operatorTest, quarter, ReturnStatus.Created);

                var categoryValues = new List<NonObligatedValue>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new NonObligatedValue((int)category, (int)category, dcf));
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

                var dataAccess = new AddNonObligatedDataAccess(database.WeeeContext);

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
    }
}
