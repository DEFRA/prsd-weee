namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using FakeItEasy;
    using System;
    using Xml.MemberRegistration;
    using Xunit;
    using System.Threading.Tasks;

    public class FetchProducerChargeTests
    {
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;
        private readonly IOnlineMarketplaceChargeDataAccess onlineMarketplaceChargeDataAccess;
        private readonly FetchProducerCharge fetchProducerCharge;

        public FetchProducerChargeTests()
        {
            producerChargeCalculatorDataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            onlineMarketplaceChargeDataAccess = A.Fake<IOnlineMarketplaceChargeDataAccess>();

            fetchProducerCharge = new FetchProducerCharge(producerChargeCalculatorDataAccess, onlineMarketplaceChargeDataAccess);
        }

        [Fact]
        public async Task GetChargeBandAmountAsync_GivenEnhancedCriteria_ShouldCallDataAccessWithCorrectParameters()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var vatRegistered = true;
            var annualTurnoverBand = AnnualTurnoverBand.Greaterthanonemillionpounds;
            var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
            var complianceYear = 2025;
            var asOfUtc = new DateTime(2025, 6, 15);

            var expectedChargeBandAmount = new ChargeBandAmount(
                Guid.NewGuid(),
                ChargeBand.A,
                competentAuthority,
                vatRegistered,
                annualTurnoverBand,
                eeePlacedOnMarketBand,
                complianceYear,
                150.00m,
                asOfUtc);

            A.CallTo(() => producerChargeCalculatorDataAccess.GetChargeBandAmountAsync(
                competentAuthority, vatRegistered, annualTurnoverBand, 
                eeePlacedOnMarketBand, complianceYear, asOfUtc))
                .Returns(expectedChargeBandAmount);

            // Act
            var result = await fetchProducerCharge.GetChargeBandAmountAsync(
                competentAuthority, vatRegistered, annualTurnoverBand, 
                eeePlacedOnMarketBand, complianceYear, asOfUtc);

            // Assert
            Assert.Equal(expectedChargeBandAmount, result);
            A.CallTo(() => producerChargeCalculatorDataAccess.GetChargeBandAmountAsync(
                competentAuthority, vatRegistered, annualTurnoverBand, 
                eeePlacedOnMarketBand, complianceYear, asOfUtc))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetChargeBandAmountAsync_GivenDifferentCriteria_ShouldReturnCorrectChargeBandAmount()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.Wales;
            var vatRegistered = false;
            var annualTurnoverBand = AnnualTurnoverBand.Lessthanorequaltoonemillionpounds;
            var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket;
            var complianceYear = 2025;
            var asOfUtc = new DateTime(2025, 3, 1);

            var expectedChargeBandAmount = new ChargeBandAmount(
                Guid.NewGuid(),
                ChargeBand.E,
                competentAuthority,
                vatRegistered,
                annualTurnoverBand,
                eeePlacedOnMarketBand,
                complianceYear,
                75.00m,
                asOfUtc);

            A.CallTo(() => producerChargeCalculatorDataAccess.GetChargeBandAmountAsync(
                competentAuthority, vatRegistered, annualTurnoverBand, 
                eeePlacedOnMarketBand, complianceYear, asOfUtc))
                .Returns(expectedChargeBandAmount);

            // Act
            var result = await fetchProducerCharge.GetChargeBandAmountAsync(
                competentAuthority, vatRegistered, annualTurnoverBand, 
                eeePlacedOnMarketBand, complianceYear, asOfUtc);

            // Assert
            Assert.Equal(expectedChargeBandAmount, result);
            Assert.Equal(75.00m, result.Amount);
        }

        [Fact]
        public async Task GetOnlineMarketplaceChargeAsync_GivenChargeExists_ShouldReturnChargeAmount()
        {
            // Arrange
            var asOfUtc = new DateTime(2025, 6, 15);
            var expectedCharge = new OnlineMarketplaceCharge(
                Guid.NewGuid(),
                13631.00m,
                new DateTime(2025, 1, 1));

            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .Returns(expectedCharge);

            // Act
            var result = await fetchProducerCharge.GetOnlineMarketplaceChargeAsync(asOfUtc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(13631.00m, result.Value);
            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetOnlineMarketplaceChargeAsync_GivenNoChargeExists_ShouldReturnNull()
        {
            // Arrange
            var asOfUtc = new DateTime(2020, 1, 1);

            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .Returns((OnlineMarketplaceCharge)null);

            // Act
            var result = await fetchProducerCharge.GetOnlineMarketplaceChargeAsync(asOfUtc);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOnlineMarketplaceChargeAsync_Given2026EffectiveDate_ShouldReturnUpliftedCharge()
        {
            // Arrange
            var asOfUtc = new DateTime(2025, 10, 15);
            var expectedCharge = new OnlineMarketplaceCharge(
                Guid.NewGuid(),
                14653.00m, // 2026 uplifted rate
                new DateTime(2025, 10, 1));

            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .Returns(expectedCharge);

            // Act
            var result = await fetchProducerCharge.GetOnlineMarketplaceChargeAsync(asOfUtc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(14653.00m, result.Value);
        }

        [Fact]
        public async Task GetOnlineMarketplaceChargeAsync_ShouldCallDataAccessWithCorrectDate()
        {
            // Arrange
            var asOfUtc = new DateTime(2025, 8, 20, 14, 30, 0);

            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .Returns((OnlineMarketplaceCharge)null);

            // Act
            await fetchProducerCharge.GetOnlineMarketplaceChargeAsync(asOfUtc);

            // Assert
            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("2025-01-01", 13631.00)]
        [InlineData("2025-06-15", 13631.00)]
        [InlineData("2025-10-01", 14653.00)]
        [InlineData("2026-01-15", 14653.00)]
        public async Task GetOnlineMarketplaceChargeAsync_GivenVariousDates_ShouldReturnCorrectCharge(string dateString, decimal expectedAmount)
        {
            // Arrange
            var asOfUtc = DateTime.Parse(dateString);
            var expectedCharge = new OnlineMarketplaceCharge(
                Guid.NewGuid(),
                expectedAmount,
                asOfUtc);

            A.CallTo(() => onlineMarketplaceChargeDataAccess.GetChargeAmountAsync(asOfUtc))
                .Returns(expectedCharge);

            // Act
            var result = await fetchProducerCharge.GetOnlineMarketplaceChargeAsync(asOfUtc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAmount, result.Value);
        }

        private static ChargeBandAmount CreateLegacyChargeBandAmount(ChargeBand chargeBand, decimal amount)
        {
            return new ChargeBandAmount(
                Guid.NewGuid(),
                chargeBand,
                CompetentAuthorityType.England,
                false,
                AnnualTurnoverBand.NotApplicable,
                EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket,
                2000, // Legacy compliance year
                amount,
                new DateTime(2000, 1, 1));
        }
    }
}
