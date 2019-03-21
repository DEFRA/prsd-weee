namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
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

        public async Task<ChargeBand?> GetChargeAmount(schemeType schmemeType, producerType producerType)
        {
            var complianceYear = int.Parse(schmemeType.complianceYear);

            var previousProducerSubmission =
                await registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, complianceYear, schmemeType.approvalNo);

            if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                previousProducerSubmission.CurrentSubmission.EEEPlacedOnMarketBandType == (int)eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(producerType);
            }

            return null;
        }
    }
}
