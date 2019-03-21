namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.DataAccess.DataAccess;

    public class ProducerAmendmentChargeCalculator : IProducerAmendmentChargeCalculator
    {
        private readonly IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public ProducerAmendmentChargeCalculator(IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator, IRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.environmentAgencyProducerChargeBandCalculator = environmentAgencyProducerChargeBandCalculator;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public ChargeBand? GetChargeAmount(schemeType schmemeType, producerType producerType)
        {
            var complianceYear = int.Parse(schmemeType.complianceYear);

            var previousProducerSubmission =
                registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, complianceYear, schmemeType.approvalNo);

            return null;
        }
    }
}
