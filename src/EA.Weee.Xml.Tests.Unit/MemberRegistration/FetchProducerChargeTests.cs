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
        private readonly FetchProducerCharge fetchProducerCharge;

        public FetchProducerChargeTests()
        {
            producerChargeCalculatorDataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();

            fetchProducerCharge = new FetchProducerCharge(producerChargeCalculatorDataAccess);
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
    }
}
