namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    public class ProducerChargeBandCalculatorChooserTests
    {
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;
        private readonly IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeBandCalculatorChooserTests()
        {
            producerChargeBandCalculator = A.Fake<IProducerChargeBandCalculator>();

            var calculators =
                new List<IProducerChargeBandCalculator>() { producerChargeBandCalculator };

            producerChargeBandCalculatorChooser = new ProducerChargeBandCalculatorChooser(calculators);
        }

        [Fact]
        public void GetProducerChargeBand_GivenCalculators_ProducerChargeCalculatorToUseShouldBeFound()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();

            producerChargeBandCalculatorChooser.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => producerChargeBandCalculator.IsMatch(schemeType, producerType)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void GetProducerChargeBand_GivenCalculatorIsFound_ChargeBandShouldBeCalculated()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();

            A.CallTo(() => producerChargeBandCalculator.IsMatch(A<schemeType>._, A<producerType>._)).Returns(true);

            producerChargeBandCalculatorChooser.GetProducerChargeBand(schemeType, producerType);

            A.CallTo(() => producerChargeBandCalculator.GetProducerChargeBand(schemeType, producerType)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetProducerChargeBand_GivenCalculatorIsFound_ChargeBandFromCalculatorShouldBeReturned()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();
            var producerCharge = new ProducerCharge();

            A.CallTo(() => producerChargeBandCalculator.IsMatch(A<schemeType>._, A<producerType>._)).Returns(true);
            A.CallTo(() => producerChargeBandCalculator.GetProducerChargeBand(A<schemeType>._, A<producerType>._)).Returns(producerCharge);

            var result = await producerChargeBandCalculatorChooser.GetProducerChargeBand(schemeType, producerType);

            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async Task GetProducerChargeBand_GivenNoCalculatorFound_ApplicationExceptionExpected()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();

            A.CallTo(() => producerChargeBandCalculator.IsMatch(A<schemeType>._, A<producerType>._)).Returns(false);

            Func<Task> act = () => producerChargeBandCalculatorChooser.GetProducerChargeBand(schemeType, producerType);

            await Assert.ThrowsAsync<ApplicationException>(act);
        }
    }
}
