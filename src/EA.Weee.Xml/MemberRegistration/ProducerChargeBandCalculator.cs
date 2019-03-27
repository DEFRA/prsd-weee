namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        private readonly IFetchProducerCharge fetchProducerCharge;

        public ProducerChargeBandCalculator(IFetchProducerCharge fetchProducerCharge)
        {
            this.fetchProducerCharge = fetchProducerCharge;
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

            return year <= 2018  && producer.status == statusType.I ? true : false;
        }
    }
}
