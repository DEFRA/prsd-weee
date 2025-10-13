namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ProducerChargeBandCalculatorTests
    {
        private readonly ProducerChargeBandCalculator producerChargeBandCalculator;
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public ProducerChargeBandCalculatorTests()
        {
            fetchProducerCharge = A.Fake<IFetchProducerCharge>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();

            producerChargeBandCalculator = new ProducerChargeBandCalculator(fetchProducerCharge, registeredProducerDataAccess);
        }

        /// <summary>
        /// This test ensures that the charge band is A when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ProducerChargeBandAShouldBeRetrieved()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 150.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.A, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                true,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 150.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 150.00m };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        /// <summary>
        /// This test ensures that the charge band is B when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ProducerChargeBandBShouldBeRetrieved()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKSCOTLAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.B, 75.00m);

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.B, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Scotland,
                true,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKSCOTLAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.B, 75.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 75.00m };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        /// <summary>
        /// This test ensures that the charge band is C when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ProducerChargeBandCShouldBeRetrieved()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKNORTHERNIRELAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.C, 35.00m);

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.C, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.NorthernIreland,
                false,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKNORTHERNIRELAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.C, 35.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 35.00m };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        /// <summary>
        /// This test ensures that the charge band is D when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ProducerChargeBandDShouldBeRetrieved()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D, 50.00m);

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.D, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                false,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ProducerChargeForChargeBandDShouldBeReturned()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D, 50.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 50.00m };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        /// <summary>
        /// This test ensures that the charge band is E when the amount of EEE placed on the market per year
        /// is less than 5 Tonnes.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ProducerChargeBandEShouldBeRetrieved()
        {
            //arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.E, 15.00m);

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.E, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                false,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ProducerChargeForChargeBandShouldBeReturned()
        {
            //arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.E, 15.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 15.00m };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Theory]
        [InlineData("2018")]
        [InlineData("2017")]
        [InlineData("2016")]
        public void IsMatch_GivenSchemeIs2018OrBeforeAndProducerInsert_TrueShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.True(result);
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        [InlineData("2021")]
        public void IsMatch_GivenSchemeIsPost2018_FalseShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_GivenProducerIsAmendment_FalseShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2019" };
            var producer = new producerType { status = statusType.A };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        /// <summary>
        /// This test ensures that for England producers, the annual turnover band is set to NotApplicable.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_EnglandProducer_AnnualTurnoverBandSetToNotApplicable()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKENGLAND, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 150.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.A, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that for Non-UK producers, the annual turnover band is set to NotApplicable.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_NonUKProducer_AnnualTurnoverBandSetToNotApplicable()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.FRANCE, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2025" };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D, 30.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.D, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.NonUK,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }
        
        /// <summary>
        /// This test ensures that the legacy method is used for compliance year 2024 
        /// and charge band A is correctly calculated for VAT registered producers with >£1M turnover and ≥5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2024_MoreThan5TEEEAndVATRegisteredAndGreaterThan1M_UsesLegacyMethodChargeBandA()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2024" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.A, 375.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.A))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.A, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(375.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.A))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test ensures that the legacy method is used for compliance year 2023 
        /// and charge band B is correctly calculated for VAT registered producers with ≤£1M turnover and ≥5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2023_MoreThan5TEEEAndVATRegisteredAndLessThan1M_UsesLegacyMethodChargeBandB()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKSCOTLAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2023" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.B, 210.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.B))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.B, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(210.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.B))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test ensures that the legacy method is used for compliance year 2022 
        /// and charge band C is correctly calculated for non-VAT registered producers with ≤£1M turnover and ≥5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2022_MoreThan5TEEEAndNotVATRegisteredAndLessThan1M_UsesLegacyMethodChargeBandC()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKNORTHERNIRELAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2022" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.C, 105.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.C))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.C, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(105.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.C))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test ensures that the legacy method is used for compliance year 2021 
        /// and charge band D is correctly calculated for non-VAT registered producers with >£1M turnover and ≥5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2021_MoreThan5TEEEAndNotVATRegisteredAndGreaterThan1M_UsesLegacyMethodChargeBandD()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, false, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2021" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.D, 150.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.D))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.D, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(150.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.D))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test ensures that the legacy method is used for compliance year 2020 
        /// and charge band E is correctly calculated for producers with <5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2020_LessThan5TEE_UsesLegacyMethodChargeBandE()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2020" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.E, 30.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.E))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.E, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(30.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.E))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test ensures that producers with edge case scenarios (VAT registered but other conditions for B band) 
        /// use the legacy method for compliance year 2019 and correctly get charge band B.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_LegacyYear2019_EdgeCaseForChargeBandB_UsesLegacyMethod()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2019" };
            var producerCharge = CreateTestProducerCharge(ChargeBand.B, 75.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.B))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ChargeBandAmount);
            Assert.Equal(ChargeBand.B, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(75.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.B))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test verifies that the exact boundary year (2024) still uses the legacy method.
        /// </summary>
        [Theory]
        [InlineData("2024")]
        [InlineData("2023")]
        [InlineData("2022")]
        [InlineData("2021")]
        [InlineData("2020")]
        [InlineData("2019")]
        public async Task GetProducerChargeBand_LegacyYears_AllUseLegacyMethod(string year)
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = year };
            var producerCharge = CreateTestProducerCharge(ChargeBand.E, 30.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.E))
                .Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.E, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(ChargeBand.E))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// This test verifies that enhanced years (2025 and beyond) use the enhanced method, not legacy.
        /// </summary>
        [Theory]
        [InlineData("2025")]
        [InlineData("2026")]
        [InlineData("2027")]
        public async Task GetProducerChargeBand_EnhancedYears_UseEnhancedMethod(string year)
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = year };
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.E, 30.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.E, result.ChargeBandAmount.ChargeBand);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                false,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket,
                int.Parse(year),
                A<DateTime>._))
                .MustHaveHappened(1, Times.Exactly);
            
            // Verify
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsyncLegacy(A<ChargeBand>._))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// Helper method to create a ProducerCharge for legacy method testing.
        /// </summary>
        private static ProducerCharge CreateTestProducerCharge(ChargeBand chargeBand, decimal amount)
        {
            var chargeBandAmount = new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2000,
                amount,
                new DateTime(2000, 1, 1));

            var producerCharge = new ProducerCharge()
            {
                ChargeBandAmount = chargeBandAmount,
                Amount = amount
            };

            // Verify the object is properly constructed
            if (producerCharge == null)
            {
                throw new InvalidOperationException("ProducerCharge could not be created");
            }
            
            if (producerCharge.ChargeBandAmount == null)
            {
                throw new InvalidOperationException("ChargeBandAmount could not be created");
            }

            return producerCharge;
        }
        
        private static ChargeBandAmount CreateTestChargeBandAmount(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2018,
                amount,
                new DateTime(2018, 1, 1));
        }

        /// <summary>
        /// Helper method to create test charge band amounts for 2026 compliance year with October 2025 effective date.
        /// </summary>
        private static ChargeBandAmount CreateTestChargeBandAmountFor2026(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.Wales,
                true,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                amount,
                new DateTime(2025, 10, 1));
        }

        /// <summary>
        /// Helper method to create test charge band amounts for 2026 compliance year for England (NotApplicable turnover).
        /// </summary>
        private static ChargeBandAmount CreateTestChargeBandAmountFor2026England(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                amount,
                new DateTime(2025, 10, 1));
        }

        /// <summary>
        /// Helper method to create test charge band amounts for 2026 compliance year for Non-UK countries (NotApplicable turnover).
        /// </summary>
        private static ChargeBandAmount CreateTestChargeBandAmountFor2026NonUK(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.NonUK,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                amount,
                new DateTime(2025, 10, 1));
        }

        private static producerType CreateProducerWithCountry(countryType countryType, annualTurnoverBandType annualTurnoverBandType, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBandType)
        {
            var producerCompany = new companyType()
            {
                companyName = "Test company",
                companyNumber = "Test CRN",
                registeredOffice = new contactDetailsContainerType()
                {
                    contactDetails = new contactDetailsType()
                    {
                        address = new addressType()
                        {
                            country = countryType
                        }
                    }
                }
            };

            var producerBusiness = new producerBusinessType()
            {
                Item = producerCompany
            };

            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType,
                VATRegistered = vatRegistered,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType,
                producerBusiness = producerBusiness,
                tradingName = "Test Trading Name", // Add trading name as fallback
                registrationNo = "TestRegistrationNo", // Add registration number for IsMatch method
                status = statusType.I // Add status for IsMatch method
            };
            return producer;
        }
    }
}
