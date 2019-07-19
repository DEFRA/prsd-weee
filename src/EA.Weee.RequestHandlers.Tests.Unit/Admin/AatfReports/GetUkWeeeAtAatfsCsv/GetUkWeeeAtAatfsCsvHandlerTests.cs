namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.DataReturns;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Reports;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;

    public class GetUkWeeeAtAatfsCsvHandlerTests
    {
        private readonly Fixture fixture;

        public GetUkWeeeAtAatfsCsvHandlerTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetUkWeeeAtAatfsCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            var request = new GetUkWeeeAtAatfsCsv(A.Dummy<int>());

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_Always_GeneratesCorrectFileName()
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateUserWithAllRights();

            var handler = new GetUkWeeeAtAatfsCsvHandler(
                authorization,
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            var request = new GetUkWeeeAtAatfsCsv(2016);

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 0));
            var result = await handler.HandleAsync(request);
            SystemTime.Unfreeze();

            // Assert
            Assert.Equal("UK WEEE received report 2016.csv", result.FileName);
        }

        [Fact]
        public void CreateWriter_Always_CreatesExpectedColumns()
        {
            // Arrange
            var handler = new GetUkWeeeAtAatfsCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            var result = handler.CreateWriter();

            // Assert
            Assert.Collection(result.ColumnTitles,
                title => Assert.Equal("Quarter", title),
                title => Assert.Equal("Category", title),
                title => Assert.Equal("B2C received for treatment (total tonnes)", title),
                title => Assert.Equal("B2C for reuse (total tonnes)", title),
                title => Assert.Equal("B2C sent to AATF/ATF (total tonnes)", title),
                title => Assert.Equal("B2B received for treatment (total tonnes)", title),
                title => Assert.Equal("B2B for reuse (total tonnes)", title),
                title => Assert.Equal("B2B sent to AATF/ATF (total tonnes)", title));
        }

        [Fact]
        public void CreateResults_Always_CreatesAResultForEachCategory()
        {
            // Arrange
            var year = (1900 + fixture.Create<int>()).ToString();
            var handler = new GetUkWeeeAtAatfsCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            var results = handler.CreateResults(A.Dummy<IEnumerable<PartialAatfReturn>>(), year);

            // Assert
            Assert.Equal(14 * 5, results.Count());
            Assert.Collection(results,
                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(QuarterType.Q1.ToString(), r.TimePeriod); },

                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(QuarterType.Q2.ToString(), r.TimePeriod); },

                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(QuarterType.Q3.ToString(), r.TimePeriod); },

                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(QuarterType.Q4.ToString(), r.TimePeriod); },

                r => { Assert.Equal(WeeeCategory.LargeHouseholdAppliances, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.SmallHouseholdAppliances, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ITAndTelecommsEquipment, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ConsumerEquipment, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.LightingEquipment, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ElectricalAndElectronicTools, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.ToysLeisureAndSports, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MedicalDevices, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.MonitoringAndControlInstruments, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.AutomaticDispensers, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.DisplayEquipment, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.CoolingApplicancesContainingRefrigerants, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.GasDischargeLampsAndLedLightSources, r.Category); Assert.Equal(year, r.TimePeriod); },
                r => { Assert.Equal(WeeeCategory.PhotovoltaicPanels, r.Category); Assert.Equal(year, r.TimePeriod); });
        }

        [Fact]
        public void CreateResults_WithNoAmounts_PopulatesAllValuesAs0()
        {
            // Arrange
            var handler = new GetUkWeeeAtAatfsCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            var results = handler.CreateResults(A.Dummy<IEnumerable<PartialAatfReturn>>(), fixture.Create<int>().ToString());

            // Assert
            Assert.Equal(0, results.Sum(r => r.B2cForTreatment));
            Assert.Equal(0, results.Sum(r => r.B2cForReuse));
            Assert.Equal(0, results.Sum(r => r.B2cForAatf));
            Assert.Equal(0, results.Sum(r => r.B2bForTreatment));
            Assert.Equal(0, results.Sum(r => r.B2bForReuse));
            Assert.Equal(0, results.Sum(r => r.B2bForAatf));
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public void CreateResults_WithSingleQuarter_PopulatesQuarterWithSum(QuarterType quarterType)
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var category = fixture.Create<WeeeCategory>();

            var received = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();
            var reused = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();
            var sentOn = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();
            var partialReturn = fixture.Build<PartialAatfReturn>()
                .With(r => r.Quarter, new Quarter(year, quarterType))
                .With(r => r.ObligatedWeeeReceivedData, received)
                .With(r => r.ObligatedWeeeReusedData, reused)
                .With(r => r.ObligatedWeeeSentOnData, sentOn)
                .Create();

            var handler = new GetUkWeeeAtAatfsCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            var results = handler.CreateResults(new List<PartialAatfReturn>() { partialReturn }, year.ToString());

            // Assert
            var result = results.SingleOrDefault(r => r.Category == category && r.TimePeriod == quarterType.ToString());
            Assert.NotNull(result);
            Assert.Equal(received.Sum(r => r.B2B), result.B2bForTreatment);
            Assert.Equal(received.Sum(r => r.B2C), result.B2cForTreatment);
            Assert.Equal(reused.Sum(r => r.B2B), result.B2bForReuse);
            Assert.Equal(reused.Sum(r => r.B2C), result.B2cForReuse);
            Assert.Equal(sentOn.Sum(r => r.B2B), result.B2bForAatf);
            Assert.Equal(sentOn.Sum(r => r.B2C), result.B2cForAatf);
        }

        [Fact]
        public void CreateResults_WithMultipleQuarters_PopulatesYearWithSum()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var category = fixture.Create<WeeeCategory>();

            var partialReturnQ1 = CreatePartialReturn(year, category, out var receivedQ1, out var reusedQ1, out var sentOnQ1);
            var partialReturnQ2 = CreatePartialReturn(year, category, out var receivedQ2, out var reusedQ2, out var sentOnQ2);
            var partialReturnQ3 = CreatePartialReturn(year, category, out var receivedQ3, out var reusedQ3, out var sentOnQ3);
            var partialReturnQ4 = CreatePartialReturn(year, category, out var receivedQ4, out var reusedQ4, out var sentOnQ4);

            var handler = new GetUkWeeeAtAatfsCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<IGetUkWeeeAtAatfsCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            var results = handler.CreateResults(new List<PartialAatfReturn>()
                {
                    partialReturnQ1,
                    partialReturnQ2,
                    partialReturnQ3,
                    partialReturnQ4
                }, year.ToString());

            // Assert
            var result = results.SingleOrDefault(r => r.Category == category && r.TimePeriod == year.ToString());
            Assert.NotNull(result);

            var receivedAll = receivedQ1.Concat(receivedQ2).Concat(receivedQ3).Concat(receivedQ4);
            Assert.Equal(receivedAll.Sum(r => r.B2B), result.B2bForTreatment);
            Assert.Equal(receivedAll.Sum(r => r.B2C), result.B2cForTreatment);

            var reusedAll = reusedQ1.Concat(reusedQ2).Concat(reusedQ3).Concat(reusedQ4);
            Assert.Equal(reusedAll.Sum(r => r.B2B), result.B2bForReuse);
            Assert.Equal(reusedAll.Sum(r => r.B2C), result.B2cForReuse);

            var sentOnAll = sentOnQ1.Concat(sentOnQ2).Concat(sentOnQ3).Concat(sentOnQ4);
            Assert.Equal(sentOnAll.Sum(r => r.B2B), result.B2bForAatf);
            Assert.Equal(sentOnAll.Sum(r => r.B2C), result.B2cForAatf);
        }

        private PartialAatfReturn CreatePartialReturn(int year, WeeeCategory category, out List<WeeeObligatedData> received, out List<WeeeObligatedData> reused, out List<WeeeObligatedData> sentOn)
        {
            received = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();

            reused = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();

            sentOn = fixture.Build<WeeeObligatedData>()
                .With(r => r.CategoryId, (int)category)
                .CreateMany().ToList();

            return fixture.Build<PartialAatfReturn>()
                .With(r => r.Quarter, new Quarter(year, QuarterType.Q1))
                .With(r => r.ObligatedWeeeReceivedData, received)
                .With(r => r.ObligatedWeeeReusedData, reused)
                .With(r => r.ObligatedWeeeSentOnData, sentOn)
                .Create();
        }
    }
}
