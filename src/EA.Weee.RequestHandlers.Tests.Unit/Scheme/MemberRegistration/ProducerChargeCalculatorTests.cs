namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using Domain.Lookup;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using Xml.MemberRegistration;
    using Xunit;

    public class ProducerChargeCalculatorTests
    {
        /// <summary>
        /// This test ensures that the calculation for an "A" producer record in a charge band with an amount that exceeds
        /// the sum of charges that already existing for the year will result in a charge with an amount that is the
        /// amount of the new charge band minus the amount from existing charges.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsAmendmentInChargeBandCosting25WithPreviousTotalChargesOf1_ReturnsChargeAmountOf24()
        {
            // Arrange
            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("65D9ADC8-B53F-4570-A1C7-F49B0503FA6A"),
                ChargeBand.A,
                25);

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A)).Returns(chargeBandAmount);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges(A<string>._, "WEE/AB1234CD", 2016)).Returns(1);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(ChargeBand.A);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";

            // Act
            ProducerCharge result = calculator.CalculateCharge(A<string>._, producer, 2016);

            // Assert
            Assert.Equal(24, result.Amount);
        }

        /// <summary>
        /// This test ensures that the calculation for an "A" producer record in a charge band with an amount that is
        /// less that the sum of charges that already existing for the year will result in a charge with an amount of 0.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsAmendmentInChargeBandCosting1WithPreviousTotalChargesOf25_ReturnsChargeAmountOf0()
        {
            // Arrange
            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("65D9ADC8-B53F-4570-A1C7-F49B0503FA6A"),
                ChargeBand.A,
                1);

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A)).Returns(chargeBandAmount);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges(A<string>._, "WEE/AB1234CD", 2016)).Returns(25);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(ChargeBand.A);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";
            
            // Act
            ProducerCharge result = calculator.CalculateCharge(A<string>._, producer, 2016);

            // Assert
            Assert.Equal(0, result.Amount);
        }

        /// <summary>
        /// This test ensures that the calculation for an "A" producer record in a charge band with no charges
        /// already existing for the year will result in a charge with an amount that is the same as the amount
        /// of the charge band.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsAmendmentInChargeBandCosting25WithNoPrevoiusCharges_ReturnsChargeAmountOf25()
        {
            // Arrange
            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("65D9ADC8-B53F-4570-A1C7-F49B0503FA6A"),
                ChargeBand.A,
                25);

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A)).Returns(chargeBandAmount);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges(A<string>._, "WEE/AB1234CD", 2016)).Returns(0);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(ChargeBand.A);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";

            // Act
            ProducerCharge result = calculator.CalculateCharge(A<string>._, producer, 2016);

            // Assert
            Assert.Equal(25, result.Amount);
        }

        /// <summary>
        /// This test ensures that the calculation for an "I" producer record will result in a charge with
        /// an amount that is the same as the amount of the charge band.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsInsertInChargeBandCosting25_ReturnsChargeAmountOf25()
        {
            ChargeBandAmount chargeBandAmount = new ChargeBandAmount(
                new Guid("65D9ADC8-B53F-4570-A1C7-F49B0503FA6A"),
                ChargeBand.A,
                25);

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A)).Returns(chargeBandAmount);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges(A<string>._, "WEE/AB1234CD", 2016)).Returns(0);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(ChargeBand.A);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.I;

            // Act
            ProducerCharge result = calculator.CalculateCharge(A<string>._, producer, 2016);

            // Assert
            Assert.Equal(25, result.Amount);
        }
   }
}