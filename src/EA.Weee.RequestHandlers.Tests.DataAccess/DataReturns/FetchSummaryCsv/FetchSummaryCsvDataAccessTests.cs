namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using RequestHandlers.DataReturns.FetchSummaryCsv;
    using Weee.DataAccess.StoredProcedure;
    using Xunit;

    public class FetchSummaryCsvDataAccessTests
    {
        /// <summary>
        /// This test ensures that an empty list of results is returned when no return
        /// data exists for the specified compliance year.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithNoReturnDataForComplianceYear_ReturnsEmptyList()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2059, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        /// <summary>
        /// This test ensures that an empty list of results is returned when no return
        /// data exists for the specified compliance year.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithUnsubmittedReturnData_ReturnsEmptyList()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        /// <summary>
        /// This test ensures that an empty list of results is returned when no return
        /// data exists for the specified scheme.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithNoReturnDataForScheme_ReturnsEmptyList()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme1 = new Scheme(organisation);
                Scheme scheme2 = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme1, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme2.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// collected from a DCF under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 0 (type 0 amounts are collected)
        /// - Source = 0 (for type 0, source 0 amounts are collcted from DCFs)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithCollectedFromDcfB2CAmount_ReturnsOneResultForQ1Type0Source0B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(0, result.Type);
                Assert.Equal(0, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// collected from a distributor under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 0 (type 0 amounts are collected)
        /// - Source = 1 (for type 0, source 1 amounts are collcted from distributors)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithCollectedFromDistributorB2CAmount_ReturnsOneResultForQ1Type0Source1B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Distributor,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(0, result.Type);
                Assert.Equal(1, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// collected from a final holder under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 0 (type 0 amounts are collected)
        /// - Source = 2 (for type 0, source 2 amounts are collcted from final holders)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithCollectedFromFinalHolderB2CAmount_ReturnsOneResultForQ1Type0Source2B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.FinalHolder,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(0, result.Type);
                Assert.Equal(2, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// sent to an AATF under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 1 (type 1 amounts are delivered)
        /// - Source = 0 (for type 1, source 0 amounts are delievered to AATFs)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithDeliveredToAatfB2CAmount_ReturnsOneResultForQ1Type1Source0B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeDeliveredAmount amount1 = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AatfDeliveryLocation("WEE/AA1111AA/ATF", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(1, result.Type);
                Assert.Equal(0, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// sent to an AE under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 1 (type 1 amounts are delivered)
        /// - Source = 1 (for type 1, source 1 amounts are delievered to AEs)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithDeliveredToAeB2CAmount_ReturnsOneResultForQ1Type1Source1B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeDeliveredAmount amount1 = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AeDeliveryLocation("WEE/AA1111AA/AE", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(1, result.Type);
                Assert.Equal(1, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount reported as
        /// EEE output under the B2C obligation type generates a single result with:
        /// - Quarter = 1
        /// - Type = 2 (type 2 amounts are output EEE)
        /// - Source = NULL (for type 2, there are no sources)
        /// - ObligationType = "B2C"
        /// - etc
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithQ1ReturnDataWithOutputB2CAmount_ReturnsOneResultForQ1Type2B2C()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                RegisteredProducer registeredProducer1 = new RegisteredProducer("WEE/AA1111AA", 2099, scheme);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                EeeOutputAmount amount1 = new EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer1);
                version.EeeOutputReturnVersion.AddEeeOutputAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(1, result.Quarter);
                Assert.Equal(2, result.Type);
                Assert.Equal(null, result.Source);
                Assert.Equal("B2C", result.ObligationType);
            }
        }

        /// <summary>
        /// This test ensures that return data consisting of a single amount reported under
        /// "Large Household Appliances" (cateogry 1) returns null, rather than 0, for all other
        /// categories.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithLargeHouseHoldAppliancesAmount_ReturnsNullForAllOtherCategories()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amount1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amount1);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.NotNull(result.Category1);
                Assert.Null(result.Category2);
                Assert.Null(result.Category3);
                Assert.Null(result.Category4);
                Assert.Null(result.Category5);
                Assert.Null(result.Category6);
                Assert.Null(result.Category7);
                Assert.Null(result.Category8);
                Assert.Null(result.Category9);
                Assert.Null(result.Category10);
                Assert.Null(result.Category11);
                Assert.Null(result.Category12);
                Assert.Null(result.Category13);
                Assert.Null(result.Category14);
            }
        }

        /// <summary>
        /// This test ensures that return data consisting of several delivered amounts
        /// reported under the same category return a single result with the amounts summed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithSeveralDeliveredAmounts_ReturnsSumOfAmounts()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeDeliveredAmount amount1 = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    100,
                    new AatfDeliveryLocation("WEE/AA1111AA/ATF", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amount1);

                WeeeDeliveredAmount amount2 = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    200,
                    new AatfDeliveryLocation("WEE/BB2222BB/ATF", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amount2);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(300, result.Category1);
            }
        }

        /// <summary>
        /// This test ensures that return data consisting of several outputs amounts
        /// reported under the same category return a single result with the amounts summed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithSeveralOutputAmounts_ReturnsSumOfAmounts()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                RegisteredProducer registeredProducer1 = new RegisteredProducer("WEE/AA1111AA", 2099, scheme);
                RegisteredProducer registeredProducer2 = new RegisteredProducer("WEE/BB2222BB", 2099, scheme);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                EeeOutputAmount amount1 = new EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    100,
                    registeredProducer1);
                version.EeeOutputReturnVersion.AddEeeOutputAmount(amount1);

                EeeOutputAmount amount2 = new EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    200,
                    registeredProducer2);
                version.EeeOutputReturnVersion.AddEeeOutputAmount(amount2);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                DataReturnSummaryCsvData result = results[0];
                Assert.NotNull(result);

                Assert.Equal(300, result.Category1);
            }
        }

        /// <summary>
        /// This test ensures that Q1 return data consisting of a single amount for
        /// each type (collected, delivered, output) and source (DCF, distributor, final
        /// holder, AATF and AE) and obligation type (B2C and B2B) generates a result for
        /// each amount, ordered as follows:
        /// - Collected from DCF
        /// - Collected from distributor
        /// - Collected from final holder,
        /// - Delivered to AATF
        /// - Delivered to AE
        /// - EEE output.
        /// Within each type/source, the B2C amount should be reported before the B2B amount.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithOneAmountOfEachTypeAndSourceAndObligationType_ReturnsResultForEachAmountWithCorrectOrdering()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                RegisteredProducer registeredProducer1 = new RegisteredProducer("WEE/AA9365AA", 2099, scheme);

                Quarter quarter = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturn = new DataReturn(scheme, quarter);

                DataReturnVersion version = new DataReturnVersion(dataReturn);

                WeeeCollectedAmount amountCollectedDcfB2C = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountCollectedDcfB2C);

                WeeeCollectedAmount amountCollectedDistributor = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Distributor,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountCollectedDistributor);

                WeeeCollectedAmount amountCollectedFinalHolder = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.FinalHolder,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountCollectedFinalHolder);

                WeeeDeliveredAmount amountDeliveredAatfB2C = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AatfDeliveryLocation("WEE/AA6845AA/ATF", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amountDeliveredAatfB2C);

                WeeeDeliveredAmount amountDeliveredAeB2C = new WeeeDeliveredAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AeDeliveryLocation("WEE/AA2658AA/AE", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amountDeliveredAeB2C);

                EeeOutputAmount amountOutputB2C = new EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer1);
                version.EeeOutputReturnVersion.AddEeeOutputAmount(amountOutputB2C);

                WeeeCollectedAmount amountCollectedDcfB2B = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2B,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                version.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountCollectedDcfB2B);

                WeeeDeliveredAmount amountDeliveredAatfB2B = new WeeeDeliveredAmount(
                    ObligationType.B2B,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AatfDeliveryLocation("WEE/AA7445AA/ATF", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amountDeliveredAatfB2B);

                WeeeDeliveredAmount amountDeliveredAeB2B = new WeeeDeliveredAmount(
                    ObligationType.B2B,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    new AeDeliveryLocation("WEE/AA3658AA/AE", string.Empty));
                version.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amountDeliveredAeB2B);

                EeeOutputAmount amountOutputB2B = new EeeOutputAmount(
                    ObligationType.B2B,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer1);
                version.EeeOutputReturnVersion.AddEeeOutputAmount(amountOutputB2B);

                wrapper.WeeeContext.DataReturns.Add(dataReturn);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn.SetCurrentVersion(version);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(10, results.Count);
                Assert.Collection(results,
                    r => { Assert.Equal(0, r.Type); Assert.Equal(0, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(0, r.Type); Assert.Equal(0, r.Source); Assert.Equal("B2B", r.ObligationType); },
                    r => { Assert.Equal(0, r.Type); Assert.Equal(1, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(0, r.Type); Assert.Equal(2, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(1, r.Type); Assert.Equal(0, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(1, r.Type); Assert.Equal(0, r.Source); Assert.Equal("B2B", r.ObligationType); },
                    r => { Assert.Equal(1, r.Type); Assert.Equal(1, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(1, r.Type); Assert.Equal(1, r.Source); Assert.Equal("B2B", r.ObligationType); },
                    r => { Assert.Equal(2, r.Type); Assert.Equal(null, r.Source); Assert.Equal("B2C", r.ObligationType); },
                    r => { Assert.Equal(2, r.Type); Assert.Equal(null, r.Source); Assert.Equal("B2B", r.ObligationType); });
            }
        }

        /// <summary>
        /// This test ensures that return data for two different quarters is reported under different results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchResultsAsync_WithReturnDataForTowQuarters_ReturnsOneResultForEachQuarter()
        {
            using (Weee.Tests.Core.Model.DatabaseWrapper wrapper = new Weee.Tests.Core.Model.DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Scheme scheme = new Scheme(organisation);

                Quarter quarter1 = new Quarter(2099, QuarterType.Q1);
                DataReturn dataReturnQ1 = new DataReturn(scheme, quarter1);

                DataReturnVersion versionQ1 = new DataReturnVersion(dataReturnQ1);

                WeeeCollectedAmount amountQ1 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                versionQ1.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountQ1);

                wrapper.WeeeContext.DataReturns.Add(dataReturnQ1);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturnQ1.SetCurrentVersion(versionQ1);
                await wrapper.WeeeContext.SaveChangesAsync();

                Quarter quarter2 = new Quarter(2099, QuarterType.Q2);
                DataReturn dataReturnQ2 = new DataReturn(scheme, quarter2);

                DataReturnVersion versionQ2 = new DataReturnVersion(dataReturnQ2);

                WeeeCollectedAmount amountQ2 = new WeeeCollectedAmount(
                    WeeeCollectedAmountSourceType.Dcf,
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m);
                versionQ2.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(amountQ2);

                wrapper.WeeeContext.DataReturns.Add(dataReturnQ2);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturnQ2.SetCurrentVersion(versionQ2);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                FetchSummaryCsvDataAccess dataAccess = new FetchSummaryCsvDataAccess(wrapper.WeeeContext);

                List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(scheme.Id, 2099);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(2, results.Count);

                Assert.Collection(results,
                    r => Assert.Equal(1, r.Quarter),
                    r => Assert.Equal(2, r.Quarter));
            }
        }
    }
}
