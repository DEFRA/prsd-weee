namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.Xml;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xml.Schemas;
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
            ChargeBandType chargeBand = A.Dummy<ChargeBandType>();

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchChargeBandAmount(chargeBand)).Returns(25);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges("WEE/AB1234CD", 2016)).Returns(1);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(chargeBand);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";

            // Act
            ProducerCharge result = calculator.CalculateCharge(producer, 2016);

            // Assert
            Assert.Equal(24, result.ChargeAmount);
        }

        /// <summary>
        /// This test ensures that the calculation for an "A" producer record in a charge band with an amount that is
        /// less that the sum of charges that already existing for the year will result in a charge with an amount of 0.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsAmendmentInChargeBandCosting1WithPreviousTotalChargesOf25_ReturnsChargeAmountOf0()
        {
            ChargeBandType chargeBand = A.Dummy<ChargeBandType>();

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchChargeBandAmount(chargeBand)).Returns(1);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges("WEE/AB1234CD", 2016)).Returns(25);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(chargeBand);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";
            
            // Act
            ProducerCharge result = calculator.CalculateCharge(producer, 2016);

            // Assert
            Assert.Equal(0, result.ChargeAmount);
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
            ChargeBandType chargeBand = A.Dummy<ChargeBandType>();

            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchChargeBandAmount(chargeBand)).Returns(25);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges("WEE/AB1234CD", 2016)).Returns(0);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(chargeBand);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.A;
            producer.registrationNo = "WEE/AB1234CD";

            // Act
            ProducerCharge result = calculator.CalculateCharge(producer, 2016);

            // Assert
            Assert.Equal(25, result.ChargeAmount);
        }

        /// <summary>
        /// This test ensures that the calculation for an "I" producer record will result in a charge with
        /// an amount that is the same as the amount of the charge band.
        /// </summary>
        [Fact]
        public void CalculateCharge_ProducerIsInsertInChargeBandCosting25_ReturnsChargeAmountOf25()
        {
            ChargeBandType chargeBand = A.Dummy<ChargeBandType>();
            
            IProducerChargeCalculatorDataAccess dataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            A.CallTo(() => dataAccess.FetchChargeBandAmount(chargeBand)).Returns(25);
            A.CallTo(() => dataAccess.FetchSumOfExistingCharges("WEE/AB1234CD", 2016)).Returns(0);

            IProducerChargeBandCalculator bandCalculator = A.Fake<IProducerChargeBandCalculator>();
            A.CallTo(() => bandCalculator.GetProducerChargeBand(A<annualTurnoverBandType>._, A<bool>._, A<eeePlacedOnMarketBandType>._))
                .Returns(chargeBand);

            ProducerChargeCalculator calculator = new ProducerChargeCalculator(dataAccess, bandCalculator);

            producerType producer = new producerType();
            producer.status = statusType.I;

            // Act
            ProducerCharge result = calculator.CalculateCharge(producer, 2016);

            // Assert
            Assert.Equal(25, result.ChargeAmount);
        }
   }
}