namespace EA.Weee.Xml.Tests.Unit.MemberUpload
{
    using Domain.Lookup;
    using Xml.MemberUpload;
    using Xunit;

    public class ProducerChargeBandCalculatorTests
    {
        /// <summary>
        /// This test ensures that the charge band is A when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
            bool vatRegistered = true;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBand result = new ProducerChargeBandCalculator().GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBand.A, result);
        }

        /// <summary>
        /// This test ensures that the charge band is B when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ReturnsChargeBandB()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = true;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBand result = new ProducerChargeBandCalculator().GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBand.B, result);
        }

        /// <summary>
        /// This test ensures that the charge band is C when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ReturnsChargeBandC()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBand result = new ProducerChargeBandCalculator().GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBand.C, result);
        }

        /// <summary>
        /// This test ensures that the charge band is D when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ReturnsChargeBandD()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;

            // Act
            ChargeBand result = new ProducerChargeBandCalculator().GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBand.D, result);
        }

        /// <summary>
        /// This test ensures that the charge band is E when the amount of EEE placed on the market per year
        /// is less than 5 Tonnes.
        /// </summary>
        [Fact]
        public void GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ReturnsChargeBandE()
        {
            // Arrange
            annualTurnoverBandType annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds;
            bool vatRegistered = false;
            eeePlacedOnMarketBandType eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;

            // Act
            ChargeBand result = new ProducerChargeBandCalculator().GetProducerChargeBand(
                annualTurnoverBand,
                vatRegistered,
                eeePlacedOnMarketBand);

            // Assert
            Assert.Equal(ChargeBand.E, result);
        }
    }
}
