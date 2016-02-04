namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgSchemeWeeeCsvAsyncTests
    {
        [Fact]
        public async void GivenSomeCollectedWeeeExists_StoredProcedureReturnsThatDataSuccessfully()
        {
            const int complianceYear = 1372;
            const ObligationType obligationType = ObligationType.B2B;
            const int collectedTonnage = 179;
            const WeeeCategory category = WeeeCategory.AutomaticDispensers;
            const QuarterType quarterType = QuarterType.Q1;

            using (var dbWrapper = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(dbWrapper.Model);

                var org = modelHelper.CreateOrganisation();

                var scheme = modelHelper.CreateScheme(org);
                scheme.ObligationType = (int)obligationType;

                var weeeCollectedReturnVersion = modelHelper.CreateWeeeCollectedReturnVersion();

                modelHelper.CreateDataReturnVersion(scheme, complianceYear, (int)quarterType, true, null, weeeCollectedReturnVersion);

                await dbWrapper.Model.SaveChangesAsync();

                var weeeCollectedAmount = modelHelper.CreateWeeeCollectedAmount(obligationType, collectedTonnage, category);

                modelHelper.CreateWeeeCollectedReturnVersionAmount(weeeCollectedAmount, weeeCollectedReturnVersion);

                await dbWrapper.Model.SaveChangesAsync();

                var results =
                    await dbWrapper.StoredProcedures.SpgSchemeWeeeCsvAsync(complianceYear, obligationType.ToString());

                Assert.NotEmpty(results.CollectedAmounts);

                var collectedAmountResult = results.CollectedAmounts.Single(ca => ca.SchemeId == scheme.Id);

                Assert.NotNull(collectedAmountResult);
                Assert.Equal((int)quarterType, collectedAmountResult.QuarterType);
                Assert.Equal(collectedTonnage, collectedAmountResult.Tonnage);
                Assert.Equal((int)category, collectedAmountResult.WeeeCategory);

                var schemeResult = results.Schemes.Single(s => s.SchemeId == scheme.Id);

                Assert.NotNull(schemeResult);
            }
        }
    }
}
