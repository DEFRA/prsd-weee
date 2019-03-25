namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public async Task<ChargeBand> GetProducerChargeBand(schemeType scheme, producerType producer)
        {
            if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return await Task.FromResult(ChargeBand.E);
            }
            else
            {
                if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && producer.VATRegistered
                    && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return await Task.FromResult(ChargeBand.A);
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && producer.VATRegistered
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return await Task.FromResult(ChargeBand.B);
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !producer.VATRegistered
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return await Task.FromResult(ChargeBand.D);
                }
                else
                {
                    return await Task.FromResult(ChargeBand.C);
                }
            }
        }

        public bool IsMatch(schemeType scheme, producerType producer)
        {
            var year = int.Parse(scheme.complianceYear);

            return year <= 2018  && producer.status == statusType.I ? true : false;
        }
    }
}
