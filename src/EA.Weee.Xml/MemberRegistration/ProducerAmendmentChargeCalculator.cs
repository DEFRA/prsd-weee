namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;
    using EA.Weee.DataAccess.DataAccess;

    public class ProducerAmendmentChargeCalculator : IProducerChargeBandCalculator
    {
        private readonly IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public ProducerAmendmentChargeCalculator(IEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator, IRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.environmentAgencyProducerChargeBandCalculator = environmentAgencyProducerChargeBandCalculator;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public async Task<ChargeBand> GetProducerChargeBand(schemeType schmemeType, producerType producerType)
        {
            var complianceYear = int.Parse(schmemeType.complianceYear);

            var previousProducerSubmission =
                await registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, complianceYear, schmemeType.approvalNo);

            var previousAmendmentCharge =
                await registeredProducerDataAccess.HasPreviousAmendmentCharge(producerType.registrationNo, complianceYear, schmemeType.approvalNo);

            if (previousProducerSubmission?.CurrentSubmission != null &&
                !previousAmendmentCharge && (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                previousProducerSubmission.CurrentSubmission.EEEPlacedOnMarketBandType == (int)eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket))
            {
                return await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(schmemeType, producerType);
            }

            return await Task.FromResult(ChargeBand.NA);
        }

        public bool IsMatch(schemeType scheme, producerType producer)
        {
            return producer.status == statusType.A;
        }
    }
}
