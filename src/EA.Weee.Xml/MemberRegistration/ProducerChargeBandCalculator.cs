namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public ChargeBand GetProducerChargeBand(producerType producerType)
        {
            if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return ChargeBand.E;
            }
            else
            {
                if (producerType.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && producerType.VATRegistered
                    && producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.A;
                }
                else if (producerType.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && producerType.VATRegistered
                         && producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.B;
                }
                else if (producerType.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !producerType.VATRegistered
                         && producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.D;
                }
                else
                {
                    return ChargeBand.C;
                }
            }
        }
    }
}
