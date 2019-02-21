namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Xunit;
    using Aatf = Domain.AatfReturn.Aatf;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

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
                var operatorTest = new Operator(organisation);
                var competentAuthority = context.UKCompetentAuthorities.FirstOrDefault();
                var aatf = new Aatf(companyName, competentAuthority, companyRegistrationNumber, AatfStatus.Approved, operatorTest);
                var @return = new Return(operatorTest, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

                context.Organisations.Add(organisation);
                context.Schemes.Add(scheme);
                context.Aatfs.Add(aatf);
                context.Returns.Add(@return);
                await context.SaveChangesAsync();
                
                var categoryValues = new List<ObligatedValue>();

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    categoryValues.Add(new ObligatedValue((int)category, (int)category, (int)category));
                }

                var obligatedWeeeRequest = new AddObligatedReceived
                {
                    AatfId = aatf.Id,
                    SchemeId = scheme.Id,
                    ReturnId = @return.Id,
                    OrganisationId = organisation.Id,
                    CategoryValues = categoryValues
                };

                var weeeReceived = new WeeeReceived(scheme.Id, aatf.Id, @return.Id);

                var weeeReceivedAmount = new List<WeeeReceivedAmount>();

                foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
                {
                    weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
                }

                await addObligatedReceivedDataAccess.Submit(weeeReceivedAmount);
                
                var thisTestObligatedWeeeArray =
                    context.WeeeReceivedAmount.Where(t => t.WeeeReceived.ReturnId == @return.Id).ToArray();

                Assert.NotNull(thisTestObligatedWeeeArray);
                Assert.NotEmpty(thisTestObligatedWeeeArray);

                foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
                {
                    var foundCategory = thisTestObligatedWeeeArray.FirstOrDefault(o => o.CategoryId == (int)category);
                    foundCategory.Should().NotBeNull();
                    var indexNum = (int)category - 1;
                    Assert.Equal(foundCategory.HouseholdTonnage, weeeReceivedAmount[indexNum].HouseholdTonnage);
                    Assert.Equal(foundCategory.NonHouseholdTonnage, weeeReceivedAmount[indexNum].NonHouseholdTonnage);
                    Assert.Equal(foundCategory.WeeeReceived.ReturnId, weeeReceivedAmount[indexNum].WeeeReceived.ReturnId);
                }
            }
        }
    }
}