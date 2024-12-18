﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports.GetUKWeeeCsv
{
    using Domain.DataReturns;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using RequestHandlers.Admin.Reports.GetUKWeeeCsv;
    using Requests.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using DomainObligationType = Domain.Obligation.ObligationType;

    public class GetUkWeeeCsvHandlerTests
    {
        /// <summary>
        /// This test ensures that the handler throws a security exception if used by
        /// a user without access to the internal area.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            GetUkWeeeCsv request = new GetUkWeeeCsv(A.Dummy<int>());

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that the handler generates a file with a name in the following
        /// format: "2016_UK_WEEE_31122016_2359.csv".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_GeneratesCorrectFileName()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            GetUkWeeeCsv request = new GetUkWeeeCsv(2016);

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 0));
            FileInfo result = await handler.HandleAsync(request);
            SystemTime.Unfreeze();

            // Assert
            Assert.Equal("2016_UK_WEEE_collected_by_PCSs_31122016_2359.csv", result.FileName);
        }

        /// <summary>
        /// This test ensures that the CSV writer contains the columns "Category", "Obligation type",
        /// "Total WEEE from DCF", "Q1 WEEE from DCF", "Q2 WEEE from DCF", "Q3 WEEE from DCF", "Q4 WEEE from DCF",
        /// "Total WEEE from distributors", "Q1 WEEE from distributors", "Q2 WEEE from distributors",
        /// "Q3 WEEE from distributors", "Q4 WEEE from distributors", "Total WEEE from final holders",
        /// "Q1 WEEE from final holders", "Q2 WEEE from final holders", "Q3 WEEE from final holders",
        /// "Q4 WEEE from final holders", "Total WEEE delivered", "Q1 WEEE delivered", "Q2 WEEE delivered",
        /// "Q3 WEEE delivered" and "Q4 WEEE delivered".
        /// </summary>
        [Fact]
        public void CreateWriter_Always_CreatesExpectedColumns()
        {
            // Arrange
            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            CsvWriter<GetUkWeeeCsvHandler.CsvResult> result = handler.CreateWriter();

            // Assert
            Assert.Collection(result.ColumnTitles,
                title => Assert.Equal("Category", title),
                title => Assert.Equal("Obligation type", title),
                title => Assert.Equal("Total WEEE from DCF (t)", title),
                title => Assert.Equal("Q1 WEEE from DCF (t)", title),
                title => Assert.Equal("Q2 WEEE from DCF (t)", title),
                title => Assert.Equal("Q3 WEEE from DCF (t)", title),
                title => Assert.Equal("Q4 WEEE from DCF (t)", title),
                title => Assert.Equal("Total WEEE from distributors (t)", title),
                title => Assert.Equal("Q1 WEEE from distributors (t)", title),
                title => Assert.Equal("Q2 WEEE from distributors (t)", title),
                title => Assert.Equal("Q3 WEEE from distributors (t)", title),
                title => Assert.Equal("Q4 WEEE from distributors (t)", title),
                title => Assert.Equal("Total WEEE from final holders (t)", title),
                title => Assert.Equal("Q1 WEEE from final holders (t)", title),
                title => Assert.Equal("Q2 WEEE from final holders (t)", title),
                title => Assert.Equal("Q3 WEEE from final holders (t)", title),
                title => Assert.Equal("Q4 WEEE from final holders (t)", title),
                title => Assert.Equal("Total WEEE delivered (t)", title),
                title => Assert.Equal("Q1 WEEE delivered (t)", title),
                title => Assert.Equal("Q2 WEEE delivered (t)", title),
                title => Assert.Equal("Q3 WEEE delivered (t)", title),
                title => Assert.Equal("Q4 WEEE delivered (t)", title));
        }

        /// <summary>
        /// This test ensures 28 CSV rows will be created representing each category and each obligation type (B2B or B2C).
        /// The 14 rows for each obligation type should be listed together, with the B2B rows appearing before the B2C rows.
        /// </summary>
        [Fact]
        public void CreateResults_Always_CreatesAResultForEachCategoryAndEachObligationType()
        {
            // Arrange
            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(A.Dummy<IEnumerable<DataReturn>>());

            // Assert
            Assert.Equal(28, results.Count());
            Assert.Collection(results,
                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(DomainObligationType.B2B, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(DomainObligationType.B2C, r.ObligationType); });
        }

        /// <summary>
        /// This test ensures that the CSV result will contain blanks rather than 0
        /// if no amounts are returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithNoAmounts_PopulatesAllValuesAsNull()
        {
            // Arrange
            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(A.Dummy<IEnumerable<DataReturn>>());

            // Assert
            GetUkWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Null(result1.DcfTotal);
            Assert.Null(result1.DcfQ1);
            Assert.Null(result1.DcfQ2);
            Assert.Null(result1.DcfQ3);
            Assert.Null(result1.DcfQ4);
            Assert.Null(result1.DistributorTotal);
            Assert.Null(result1.DistributorQ1);
            Assert.Null(result1.DistributorQ2);
            Assert.Null(result1.DistributorQ3);
            Assert.Null(result1.DistributorQ4);
            Assert.Null(result1.FinalHolderTotal);
            Assert.Null(result1.FinalHolderQ1);
            Assert.Null(result1.FinalHolderQ2);
            Assert.Null(result1.FinalHolderQ3);
            Assert.Null(result1.FinalHolderQ4);
            Assert.Null(result1.DeliveredTotal);
            Assert.Null(result1.DeliveredQ1);
            Assert.Null(result1.DeliveredQ2);
            Assert.Null(result1.DeliveredQ3);
            Assert.Null(result1.DeliveredQ4);
        }

        /// <summary>
        /// This test ensures that the CSV result will contain 0 rather than null
        /// where a 0 value has been provided in a data return for am amount
        /// collected from DCF.
        /// </summary>
        [Fact]
        public void CreateResults_WithQ1DcfCollectedAmountOfZero_PopulatesDcfQ1AndDcfTotalWithZero()
        {
            // Arrange
            DataReturn dataReturn = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            WeeeCollectedAmount weeeCollectedAmount = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Dcf,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                0);

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);

            dataReturn.SetCurrentVersion(dataReturnVersion);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(0, result.DcfQ1);
            Assert.Null(result.DcfQ2);
            Assert.Null(result.DcfQ3);
            Assert.Null(result.DcfQ4);
            Assert.Equal(0, result.DcfTotal);
        }

        /// <summary>
        /// This test ensures that the CSV result will contain 0 rather than null
        /// where a 0 value has been provided in a data return for am amount
        /// collected from a distributor.
        /// </summary>
        [Fact]
        public void CreateResults_WithQ1DistibutorCollectedAmountOfZero_PopulatesDistributorQ1AndDistributorTotalWithZero()
        {
            // Arrange
            DataReturn dataReturn = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            WeeeCollectedAmount weeeCollectedAmount = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Distributor,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                0);

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);

            dataReturn.SetCurrentVersion(dataReturnVersion);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(0, result.DistributorQ1);
            Assert.Null(result.DistributorQ2);
            Assert.Null(result.DistributorQ3);
            Assert.Null(result.DistributorQ4);
            Assert.Equal(0, result.DistributorTotal);
        }

        /// <summary>
        /// This test ensures that the CSV result will contain 0 rather than null
        /// where a 0 value has been provided in a data return for am amount
        /// collected from a final holder.
        /// </summary>
        [Fact]
        public void CreateResults_WithQ1FinalHolderCollectedAmountOfZero_PopulatesFinalHolderQ1AndFinalHolderTotalWithZero()
        {
            // Arrange
            DataReturn dataReturn = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            WeeeCollectedAmount weeeCollectedAmount = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.FinalHolder,
                Domain.Obligation.ObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                0);

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);

            dataReturn.SetCurrentVersion(dataReturnVersion);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(0, result.FinalHolderQ1);
            Assert.Null(result.FinalHolderQ2);
            Assert.Null(result.FinalHolderQ3);
            Assert.Null(result.FinalHolderQ4);
            Assert.Equal(0, result.FinalHolderTotal);
        }

        /// <summary>
        /// This test ensures that the CSV result will contain 0 rather than null
        /// where a 0 value has been provided in a data return for am amount
        /// delivered.
        /// </summary>
        [Fact]
        public void CreateResults_WithQ1DeliveredAmountOfZero_PopulatesDeliveredQ1AndDeliveredTotalWithZero()
        {
            // Arrange
            DataReturn dataReturn = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            WeeeDeliveredAmount weeeDeliveredAmount = new WeeeDeliveredAmount(
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                0,
                new AatfDeliveryLocation("WEE/AA1234AA/ATF", "Facility Name"));

            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount);

            dataReturn.SetCurrentVersion(dataReturnVersion);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(0, result.DeliveredQ1);
            Assert.Null(result.DeliveredQ2);
            Assert.Null(result.DeliveredQ3);
            Assert.Null(result.DeliveredQ4);
            Assert.Equal(0, result.DeliveredTotal);
        }

        /// <summary>
        /// This test ensures that the CSV result will populate the DCF Q1 value
        /// with the sum of amounts from DCF Q1.
        /// </summary>
        [Fact]
        public void CreateResults_WithSeveralQ1DcfCollectedAmounts_PopulatesDcfQ1WithSum()
        {
            // Arrange
            DataReturn dataReturn1 = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion1 = new DataReturnVersion(dataReturn1);

            WeeeCollectedAmount weeeCollectedAmount1 = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Dcf,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                100);

            dataReturnVersion1.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount1);

            dataReturn1.SetCurrentVersion(dataReturnVersion1);

            DataReturn dataReturn2 = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            DataReturnVersion dataReturnVersion2 = new DataReturnVersion(dataReturn2);

            WeeeCollectedAmount weeeCollectedAmount2 = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Dcf,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                200);

            dataReturnVersion2.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount2);

            dataReturn2.SetCurrentVersion(dataReturnVersion2);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn1, dataReturn2 });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(300, result.DcfQ1);
        }

        /// <summary>
        /// This test ensures that the CSV result will populate the DCF total value
        /// with the sum of amounts from the four quarters.
        /// </summary>
        [Fact]
        public void CreateResults_WithSeveralDcfCollectedAmountsInDifferentQuarters_PopulatesDcfTotalWithSum()
        {
            // Arrange
            DataReturn dataReturn1 = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q2));

            DataReturnVersion dataReturnVersion1 = new DataReturnVersion(dataReturn1);

            WeeeCollectedAmount weeeCollectedAmount1 = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Dcf,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                100);

            dataReturnVersion1.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount1);

            dataReturn1.SetCurrentVersion(dataReturnVersion1);

            DataReturn dataReturn2 = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q4));

            DataReturnVersion dataReturnVersion2 = new DataReturnVersion(dataReturn2);

            WeeeCollectedAmount weeeCollectedAmount2 = new WeeeCollectedAmount(
                WeeeCollectedAmountSourceType.Dcf,
                DomainObligationType.B2C,
                WeeeCategory.LargeHouseholdAppliances,
                200);

            dataReturnVersion2.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount2);

            dataReturn2.SetCurrentVersion(dataReturnVersion2);

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetUkWeeeCsvHandler.CsvResult> results = handler.CreateResults(new List<DataReturn>() { dataReturn1, dataReturn2 });

            // Assert
            GetUkWeeeCsvHandler.CsvResult result = results
                .Single(r => r.Category == WeeeCategory.LargeHouseholdAppliances && r.ObligationType == DomainObligationType.B2C);
            Assert.NotNull(result);

            Assert.Equal(300, result.DcfTotal);
        }

        [Fact]
        public void CreateResults_ForDataReturnWithNullCurrentDataReturnVersion_DoesNotIncludeDataReturnItemsInResults()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            var dataReturn = new DataReturn(A.Fake<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));

            // Act
            var result = handler.CreateResults(new List<DataReturn> { dataReturn });

            // Assert
            Assert.All(result, r => Assert.Null(r.DcfQ1));
            Assert.All(result, r => Assert.Null(r.DistributorQ1));
            Assert.All(result, r => Assert.Null(r.FinalHolderQ1));
            Assert.All(result, r => Assert.Null(r.DeliveredQ1));
        }

        [Fact]
        public void CreateResults_ForDataReturnVersionWithNullWeeeCollectedDataReturnVersion_DoesNotIncludeWeeeCollectedDataReturnVersionItemsInResults()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            var dataReturn = new DataReturn(A.Fake<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));
            var dataReturnVersion = new DataReturnVersion(dataReturn, null, A.Dummy<WeeeDeliveredReturnVersion>(), A.Dummy<EeeOutputReturnVersion>());
            dataReturn.SetCurrentVersion(dataReturnVersion);

            // Act
            var result = handler.CreateResults(new List<DataReturn> { dataReturn });

            // Assert
            Assert.All(result, r => Assert.Null(r.DcfQ1));
            Assert.All(result, r => Assert.Null(r.DistributorQ1));
            Assert.All(result, r => Assert.Null(r.FinalHolderQ1));
        }

        [Fact]
        public void CreateResults_ForDataReturnVersionWithNullWeeeDeliveredDataReturnVersion_DoesNotIncludeWeeeDeliveredDataReturnVersionItemsInResults()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetUkWeeeCsvHandler handler = new GetUkWeeeCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            var dataReturn = new DataReturn(A.Fake<Domain.Scheme.Scheme>(), new Quarter(2016, QuarterType.Q1));
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(), null, A.Dummy<EeeOutputReturnVersion>());
            dataReturn.SetCurrentVersion(dataReturnVersion);

            // Act
            var result = handler.CreateResults(new List<DataReturn> { dataReturn });

            // Assert
            Assert.All(result, r => Assert.Null(r.DeliveredQ1));
        }
    }
}
