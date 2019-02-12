namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;
    using EA.Weee.Tests.Core.Model;
    using Xunit;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;
    using Aatf = Domain.AatfReturn.Aatf;
    using Return = Domain.AatfReturn.Return;
    using EA.Weee.RequestHandlers.AatfReturn.Obligatesd;
    using FluentAssertions;
    using System.Data.Entity;

    public class ObligatedReceivedWeeeIntegration
    {
        [Fact]
        public async Task CanCreateWeeeReceivedAmountEntry()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var addObligatedReceivedDataAccess = new AddObligatedReceivedDataAccess(context);

                var companyName = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string companyRegistrationNumber = "ABC12345";

                var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);
                var scheme = new Scheme(organisation);

                context.Organisations.Add(organisation);
                context.Schemes.Add(scheme);
                await context.SaveChangesAsync();

                var allOrganisations = context.Organisations.ToList();
                var allSchemes = context.Schemes.ToList();
                var schemeId = (await context.Schemes.FirstOrDefaultAsync(s => s.OrganisationId == organisation.Id)).Id;
                var aatfId = (await context.Aatfs
                    .Include(c => c.Operator)
                    .FirstOrDefaultAsync(a => a.Operator.Organisation.Id == organisation.Id)).Id;

                var operatorTest = new Operator(organisation);
                var quarter = new Quarter(2019, QuarterType.Q1);
                var aatfReturn = new Return(operatorTest, quarter, ReturnStatus.Created);

                var categoryValues = new List<ObligatedReceivedValue>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedReceivedValue((int)category, (int)category, (int)category));
                }

                var obligatedWeeeRequest = new AddObligatedReceived
                {
                    ReturnId = aatfReturn.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var weeeReceived = new WeeeReceived(schemeId, aatfId, aatfReturn.Id);

                var weeeReceivedAmounts = new List<WeeeReceivedAmount>();

                foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
                {
                    weeeReceivedAmounts.Add(new WeeeReceivedAmount(weeeReceived, categoryValue.CategoryId,categoryValue.HouseholdTonnage,categoryValue.NonHouseholdTonnage));
                }

                await addObligatedReceivedDataAccess.Submit(weeeReceivedAmounts);

                var thisTestObligatedWeeeArray =
                    context.AatfWeeReceivedAmount.Where(t => t.WeeeReceived.ReturnId == aatfReturn.Id).ToArray();

                Assert.NotNull(thisTestObligatedWeeeArray);
                Assert.NotEmpty(thisTestObligatedWeeeArray);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    var foundCategory = thisTestObligatedWeeeArray.FirstOrDefault(o => o.CategoryId == (int)category);
                    foundCategory.Should().NotBeNull();
                    var indexNum = (int)category - 1;
                    Assert.Equal(foundCategory.HouseholdTonnage, weeeReceivedAmounts[indexNum].HouseholdTonnage);
                    Assert.Equal(foundCategory.NonHouseholdTonnage, weeeReceivedAmounts[indexNum].NonHouseholdTonnage);
                    Assert.Equal(foundCategory.WeeeReceived.ReturnId, weeeReceivedAmounts[indexNum].WeeeReceived.ReturnId);
                }
            }
        }
    }
}