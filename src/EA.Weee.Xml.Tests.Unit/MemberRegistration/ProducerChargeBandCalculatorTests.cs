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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
                AnnualTurnoverBand.NotApplicable, // England - should be NotApplicable
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
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
            var scheme = new schemeType() { complianceYear = "2018" };
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
                AnnualTurnoverBand.NotApplicable, // Non-UK - should be NotApplicable
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025, // Compliance year < 2025 is now using 2025 compliance year
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that the charge band calculation works correctly for compliance year 2026 
        /// with effective date of October 1, 2025 for Wales producers.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_ComplianceYear2026_WalesProducer_WithOctober2025EffectiveDate()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKWALES, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2026" };
            var chargeBandAmount = CreateTestChargeBandAmountFor2026(ChargeBand.A, 445.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.A, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(445.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                true,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that the charge band calculation works correctly for compliance year 2026 
        /// with effective date of October 1, 2025 for Scotland producers.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_ComplianceYear2026_ScotlandProducer_WithOctober2025EffectiveDate()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKSCOTLAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2026" };
            var chargeBandAmount = CreateTestChargeBandAmountFor2026(ChargeBand.B, 210.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.B, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(210.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Scotland,
                true,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that the charge band calculation works correctly for compliance year 2026 
        /// with effective date of October 1, 2025 for England producers (annual turnover should be NotApplicable).
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_ComplianceYear2026_EnglandProducer_WithOctober2025EffectiveDate()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKENGLAND, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2026" };
            var chargeBandAmount = CreateTestChargeBandAmountFor2026England(ChargeBand.A, 806.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.A, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(806.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that the charge band calculation works correctly for compliance year 2026 
        /// with effective date of October 1, 2025 for Non-UK producers (annual turnover should be NotApplicable).
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_ComplianceYear2026_NonUKProducer_WithOctober2025EffectiveDate()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.FRANCE, annualTurnoverBandType.Greaterthanonemillionpounds, true, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2026" };
            var chargeBandAmount = CreateTestChargeBandAmountFor2026NonUK(ChargeBand.D, 32.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.D, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(32.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.NonUK,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2026,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        /// <summary>
        /// This test ensures that the charge band calculation works correctly for compliance year 2026 
        /// with effective date of October 1, 2025 for Northern Ireland producers with less than 5T EEE.
        /// </summary>
        [Fact]
        public async Task GetProducerChargeBand_ComplianceYear2026_NorthernIrelandProducer_LessThan5TEEEPlacedOnMarket()
        {
            // Arrange
            var producer = CreateProducerWithCountry(countryType.UKNORTHERNIRELAND, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket);
            var scheme = new schemeType() { complianceYear = "2026" };
            var chargeBandAmount = CreateTestChargeBandAmountFor2026(ChargeBand.E, 30.00m);
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(ChargeBand.E, result.ChargeBandAmount.ChargeBand);
            Assert.Equal(30.00m, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.NorthernIreland,
                false,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket,
                2026,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
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
                producerBusiness = producerBusiness
            };
            return producer;
        }
    }
}
