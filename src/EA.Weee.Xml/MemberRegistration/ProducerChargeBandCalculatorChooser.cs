namespace EA.Weee.Xml.MemberRegistration
{
    public class ProducerChargeBandCalculatorChooser : IProducerChargeBandCalculatorChooser
    {
        private readonly IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeBandCalculatorChooser(IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator, IProducerChargeBandCalculator producerChargeBandCalculator)
        {
            this.environmentAgencyProducerChargeBandCalculator = environmentAgencyProducerChargeBandCalculator;
            this.producerChargeBandCalculator = producerChargeBandCalculator;
        }

        public IProducerChargeBandCalculator GetCalculator(schemeType scheme, producerType producer, int complianceYear)
        {
            if (complianceYear > 2018) 
            {
                return environmentAgencyProducerChargeBandCalculator;
            }

            return producerChargeBandCalculator;
        }
    }
}
