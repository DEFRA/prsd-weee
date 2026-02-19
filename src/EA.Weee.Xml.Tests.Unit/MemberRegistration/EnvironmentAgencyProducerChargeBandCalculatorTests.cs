namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class EnvironmentAgencyProducerChargeBandCalculatorTests
    {
        private readonly EnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        // Online Marketplace charge amounts
        private const decimal OmpCharge2025 = 13631m;
        private const decimal OmpCharge2026 = 14653m;

        public EnvironmentAgencyProducerChargeBandCalculatorTests()
        {
            fetchProducerCharge = A.Fake<IFetchProducerCharge>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();

            environmentAgencyProducerChargeBandCalculator = new EnvironmentAgencyProducerChargeBandCalculator(fetchProducerCharge, registeredProducerDataAccess);
        }

        [Fact]
        public async Task GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.E, 30.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 30.00m };

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false);
            var scheme = new schemeType() { complianceYear = "2025" };
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Fact]
        public async Task GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ChargeBandEShouldBeRetrieved()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.E, 30.00m);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, false);
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._,
                A<bool>._,
                A<AnnualTurnoverBand>._,
                EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 445.00m };

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_VATRegistered_CorrectParametersShouldBeUsed()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable, // England - annual turnover not applicable
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D, 30.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 30.00m };

            var producer = SetUpProducer(countryType.FRANCE, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Fact]
        public async Task GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_VATRegistered_CorrectParametersShouldBeUsed()
        {
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D, 30.00m);

            var producer = SetUpProducer(countryType.FRANCE, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.NonUK,
                true,
                AnnualTurnoverBand.NotApplicable, // Non-UK - annual turnover not applicable
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKWales_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 445.00m };

            var producer = SetUpProducer(countryType.UKWALES, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKWales_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_CorrectParametersShouldBeUsed()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);

            var producer = SetUpProducer(countryType.UKWALES, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Wales,
                true,
                AnnualTurnoverBand.Greaterthanonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEPlacedonmarket_LessthanonemillionpoundsTurnover_VATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.B, 210.00m);
            var expectedProducerCharge = new ProducerCharge() { ChargeBandAmount = chargeBandAmount, Amount = 210.00m };

            var producer = SetUpProducer(countryType.UKSCOTLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };
            
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(expectedProducerCharge.Amount, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }

        [Fact]
        public async Task GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEPlacedonmarket_LessthanonemillionpoundsTurnover_VATRegistered_CorrectParametersShouldBeUsed()
        {
            //Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.B, 210.00m);

            var producer = SetUpProducer(countryType.UKSCOTLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Lessthanorequaltoonemillionpounds, true);
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._, 
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                CompetentAuthorityType.Scotland,
                true,
                AnnualTurnoverBand.Lessthanorequaltoonemillionpounds,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                A<DateTime>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_UKEngland_AppliesAdditionalFee()
        {
            // Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A2, 750.00m);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .Returns(OmpCharge2025);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(750.00m + OmpCharge2025, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_NonUK_AppliesAdditionalFee()
        {
            // Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.D3, 375.00m);

            var producer = SetUpProducer(countryType.FRANCE, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .Returns(OmpCharge2025);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(375.00m + OmpCharge2025, result.Amount);
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_2026_AppliesUpliftedFee()
        {
            // Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A2, 806.00m, 2026);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2026" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .Returns(OmpCharge2026);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(806.00m + OmpCharge2026, result.Amount);
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_Wales_DoesNotApplyAdditionalFee()
        {
            // Arrange - Wales producers should NOT get OMP fee (only England and NonUK)
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);

            var producer = SetUpProducer(countryType.UKWALES, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(445.00m, result.Amount); // No OMP fee added
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_Scotland_DoesNotApplyAdditionalFee()
        {
            // Arrange - Scotland producers should NOT get OMP fee (only England and NonUK)
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);

            var producer = SetUpProducer(countryType.UKSCOTLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(445.00m, result.Amount); // No OMP fee added
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_NorthernIreland_DoesNotApplyAdditionalFee()
        {
            // Arrange - Northern Ireland producers should NOT get OMP fee (only England and NonUK)
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A, 445.00m);

            var producer = SetUpProducer(countryType.UKNORTHERNIRELAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(445.00m, result.Amount); // No OMP fee added
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetProducerChargeBand_NotOnlineMarketplace_DoesNotApplyAdditionalFee()
        {
            // Arrange - Direct selling should NOT get OMP fee
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A2, 750.00m);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.DirectSellingtoEndUser;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(750.00m, result.Amount); // No OMP fee added
            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetProducerChargeBand_OnlineMarketplace_NoChargeInDatabase_DoesNotAddFee()
        {
            // Arrange
            var chargeBandAmount = CreateTestChargeBandAmount(ChargeBand.A2, 750.00m);

            var producer = SetUpProducer(countryType.UKENGLAND, eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket, annualTurnoverBandType.Greaterthanonemillionpounds, true);
            producer.sellingTechnique = sellingTechniqueType.OnlineMarketplace;
            var scheme = new schemeType() { complianceYear = "2025" };

            A.CallTo(() => fetchProducerCharge.GetChargeBandAmountAsync(
                A<CompetentAuthorityType>._, A<bool>._, A<AnnualTurnoverBand>._,
                A<EEEPlacedOnMarketBand>._, A<int>._, A<DateTime>._))
                .Returns(chargeBandAmount);

            A.CallTo(() => fetchProducerCharge.GetOnlineMarketplaceChargeAsync(A<DateTime>._))
                .Returns((decimal?)null);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(scheme, producer);

            // Assert
            Assert.Equal(750.00m, result.Amount); // No OMP fee added when null returned
        }

        [Theory]
        [InlineData("2018")]
        [InlineData("2017")]
        [InlineData("2016")]
        public void IsMatch_GivenSchemeIs2018OrBeforeAndProducerInsert_FalseShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        [InlineData("2021")]
        public void IsMatch_GivenSchemeIsPost2018_TrueShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.True(result);
        }

        [Fact]
        public void IsMatch_GivenProducerIsAmendment_FalseShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2019" };
            var producer = new producerType { status = statusType.A };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_Amendment_NoPreviousSubmission_TrueShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2020" };
            var producerType = new producerType { status = statusType.A };
            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(A<string>._, A<int>._, A<string>._)).Returns((RegisteredProducer)null);

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producerType);

            Assert.True(result);
        }

        [Fact]
        public void IsMatch_Amendment_HasPreviousSubmission_FalseShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2020" };
            var producerType = new producerType { status = statusType.A };
            var registeredProducer = new RegisteredProducer(A.Dummy<string>(), A.Dummy<int>(), A.Dummy<Scheme>());

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration("ABC", 2020, "ABC/WWW"))
                .Returns(registeredProducer);

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producerType);

            Assert.False(result);
        }

        private static producerType SetUpProducer(countryType countryType, eeePlacedOnMarketBandType eeePlacedOnMarketBandType, annualTurnoverBandType annualTurnoverBandType, bool vatRegistered)
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

        private static ChargeBandAmount CreateTestChargeBandAmount(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2025,
                amount,
                new DateTime(2025, 1, 1));
        }

        private static ChargeBandAmount CreateTestChargeBandAmount(ChargeBand chargeBand, decimal amount, int complianceYear = 2025)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                true,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                complianceYear,
                amount,
                new DateTime(complianceYear, 1, 1));
        }
    }
}