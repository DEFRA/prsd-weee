namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using Domain.Lookup;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Xml.MemberRegistration;
    using Xunit;

    public class ProducerChargeCalculatorTests
    {
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;
        private readonly ProducerChargeCalculator producerChargeCalculator;

        public ProducerChargeCalculatorTests()
        {
            producerChargeCalculatorDataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();
            producerChargeBandCalculatorChooser = A.Fake<IProducerChargeBandCalculatorChooser>();

            producerChargeCalculator = new ProducerChargeCalculator(producerChargeCalculatorDataAccess, producerChargeBandCalculatorChooser);
        }

        [Fact]
        public void CalculateCharge_GivenProducerChargeProducerChargeBandChooserShouldBeCalled()
        {
            var producer = new producerType();
            var scheme = new schemeType();

            var result = producerChargeCalculator.CalculateCharge(scheme, producer);

            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(scheme, producer)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void CalculateCharge_GivenProducerChargeBand_ProducerChargeBandShouldBeRetrieved()
        {
            var producer = new producerType();
            var scheme = new schemeType();

            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(scheme, producer)).Returns(ChargeBand.A);

            var result = producerChargeCalculator.CalculateCharge(scheme, producer);

            A.CallTo(() => producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(ChargeBand.A)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void CalculateCharge_GivenChargeBand_ReturnsProducerChargeAmount()
        {
            var producer = new producerType();
            var scheme = new schemeType();

            var chargeBandAmount = new ChargeBandAmount(Guid.Empty, ChargeBand.A, 25);
            A.CallTo(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).Returns(ChargeBand.A);
            A.CallTo(() => producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(A<ChargeBand>._)).Returns(chargeBandAmount);

            var result = producerChargeCalculator.CalculateCharge(scheme, producer);

            Assert.Equal(25, result.Amount);
            Assert.Equal(chargeBandAmount, result.ChargeBandAmount);
        }
    }
}