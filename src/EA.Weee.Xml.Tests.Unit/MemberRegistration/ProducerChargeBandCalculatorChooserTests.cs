namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;
    public class ProducerChargeBandCalculatorChooserTests
    {
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;
        private readonly IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeBandCalculatorChooserTests()
        {
            environmentAgencyProducerChargeBandCalculator = A.Fake<IEnvironmentAgencyProducerChargeBandCalculator>();
            producerChargeBandCalculator = A.Fake<IProducerChargeBandCalculator>();
            producerChargeBandCalculatorChooser = new ProducerChargeBandCalculatorChooser(environmentAgencyProducerChargeBandCalculator, producerChargeBandCalculator);
        }

        [Fact]
        public void ChoosePost2018_CalculatorForCalculatingChargeBands_Returns_EnvironmentAgencyProducerChargeBandCalculator()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();
         
            var calculatorToUse = producerChargeBandCalculatorChooser.GetCalculator(schemeType, producerType, 2019);
            
            Assert.Equal(calculatorToUse, environmentAgencyProducerChargeBandCalculator);
        }

        [Fact]
        public void ChoosePre2018_CalculatorForCalculatingChargeBands_Returns_ProducerChargeBandCalculator()
        {
            var producerType = A.Fake<producerType>();
            var schemeType = A.Fake<schemeType>();

            var calculatorToUse = producerChargeBandCalculatorChooser.GetCalculator(schemeType, producerType, 2018);

            Assert.Equal(calculatorToUse, producerChargeBandCalculator);
        }
    }
}
