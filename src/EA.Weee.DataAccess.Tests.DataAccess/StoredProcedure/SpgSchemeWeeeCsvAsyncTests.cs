namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgSchemeWeeeCsvAsyncTests
    {
        [Fact]
        public async Task GivenSomeCollectedWeeeExists_StoredProcedureReturnsThatDataSuccessfully()
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
                    await dbWrapper.StoredProcedures.SpgSchemeWeeeCsvAsync(complianceYear, null, obligationType.ToString());

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

        /// <summary>
        /// This test ensures that when no scheme ID is provided to the stored procedure, results
        /// for all scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithoutSchemeFilter_ReturnsResultsForAllSchemes()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = wrapper.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == "EA");
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.WeeeCollectedAmount amount1 = new Domain.DataReturns.WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);

                version1.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturnVersions.Add(version1);

                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 2

                Domain.Scheme.Scheme scheme2 = new Domain.Scheme.Scheme(organisation);
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395", ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.WeeeCollectedAmount amount2 = new Domain.DataReturns.WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);

                version2.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount2);

                wrapper.WeeeContext.DataReturnVersions.Add(version2);

                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn2.SetCurrentVersion(version2);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                SpgSchemeWeeeCsvResult result = await wrapper.WeeeContext.StoredProcedures.SpgSchemeWeeeCsvAsync(
                    2099,
                    null,
                    "B2C");

                // Assert
                Assert.NotNull(result);

                Assert.NotNull(result.Schemes);
                Assert.Equal(2, result.Schemes.Count);

                Assert.NotNull(result.CollectedAmounts);
                Assert.Equal(2, result.CollectedAmounts.Count);
            }
        }

        /// <summary>
        /// This test ensures that when a scheme ID is provided to the stored procedure, only results
        /// for that scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSchemeFilter_ReturnsResultsForSelectedSchemeOnly()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = wrapper.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == "EA");
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.WeeeCollectedAmount amount1 = new Domain.DataReturns.WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);

                version1.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturnVersions.Add(version1);

                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 2

                Domain.Scheme.Scheme scheme2 = new Domain.Scheme.Scheme(organisation);
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395", ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.WeeeCollectedAmount amount2 = new Domain.DataReturns.WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);

                version2.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount2);

                wrapper.WeeeContext.DataReturnVersions.Add(version2);

                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn2.SetCurrentVersion(version2);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                SpgSchemeWeeeCsvResult result = await wrapper.WeeeContext.StoredProcedures.SpgSchemeWeeeCsvAsync(
                    2099,
                    scheme1.Id,
                    "B2C");

                // Assert
                Assert.NotNull(result);

                Assert.NotNull(result.Schemes);
                Assert.Equal(1, result.Schemes.Count);
                Assert.Equal(scheme1.Id, result.Schemes[0].SchemeId);

                Assert.NotNull(result.CollectedAmounts);
                Assert.Equal(1, result.CollectedAmounts.Count);
                Assert.Equal(scheme1.Id, result.CollectedAmounts[0].SchemeId);
            }
        }
    }
}
