namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Xml.MemberRegistration;

    public class MigrationProducerAmendmentChargeCalculator : IMigrationChargeBandCalculator
    {
        private readonly IMigrationEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IMigrationRegisteredProducerDataAccess registeredProducerDataAccess;
        private readonly IFetchProducerCharge fetchProducerCharge;

        public MigrationProducerAmendmentChargeCalculator(IMigrationEnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator, IMigrationRegisteredProducerDataAccess registeredProducerDataAccess, IFetchProducerCharge fetchProducerCharge)
        {
            this.environmentAgencyProducerChargeBandCalculator = environmentAgencyProducerChargeBandCalculator;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
            this.fetchProducerCharge = fetchProducerCharge;
        }

        public async Task<ProducerCharge> GetProducerChargeBand(schemeType schmemeType, producerType producerType, MemberUpload memberUpload)
        {
            var complianceYear = int.Parse(schmemeType.complianceYear);

            var previousProducerSubmission =
                registeredProducerDataAccess.GetProducerRegistration(producerType.registrationNo, complianceYear, schmemeType.approvalNo, memberUpload);

            var previousAmendmentCharge =
                registeredProducerDataAccess.HasPreviousAmendmentCharge(producerType.registrationNo, complianceYear, schmemeType.approvalNo, memberUpload);

            var chargeband = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(schmemeType, producerType, memberUpload);

            if (previousProducerSubmission != null)
            {
                if (!previousAmendmentCharge && (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                                                 previousProducerSubmission.EEEPlacedOnMarketBandType == (int)eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket))
                {
                    return chargeband;
                }
            }

            return new ProducerCharge()
            {
                ChargeBandAmount = chargeband.ChargeBandAmount,
                Amount = 0
            };
        }

        public bool IsMatch(schemeType scheme, producerType producer, MemberUpload upload, string name)
        {
            var year = int.Parse(scheme.complianceYear);
            var previousProducerSubmission =
                registeredProducerDataAccess.GetProducerRegistrationForInsert(producer.registrationNo, year, scheme.approvalNo, upload, name);

            return producer.status == statusType.A && (previousProducerSubmission != null);
        }
    }
}
