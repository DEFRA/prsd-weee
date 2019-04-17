namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.Lookup;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        
        public ProducerChargeBandCalculator(IFetchProducerCharge fetchProducerCharge, 
            IRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.fetchProducerCharge = fetchProducerCharge;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public async Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer)
        {
            ChargeBand band;

            if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                band = ChargeBand.E;
            }
            else
            {
                if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && producer.VATRegistered
                    && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    band = ChargeBand.A;
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && producer.VATRegistered
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    band = ChargeBand.B;
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !producer.VATRegistered
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    band = ChargeBand.D;
                }
                else
                {
                    band = ChargeBand.C;
                }
            }

            return await fetchProducerCharge.GetCharge(band);
        }

        public bool IsMatch(schemeType scheme, producerType producer)
        {
            var year = int.Parse(scheme.complianceYear);
            var previousProducerSubmission = Task.Run(() => registeredProducerDataAccess.GetProducerRegistration(producer.registrationNo, year, scheme.approvalNo)).Result;

            if (year <= 2018)
            {
                if (producer.status == statusType.I)
                {
                    return true;
                }
                if (producer.status == statusType.A && previousProducerSubmission == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
