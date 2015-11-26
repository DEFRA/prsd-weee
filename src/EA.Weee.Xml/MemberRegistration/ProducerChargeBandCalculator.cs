namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;    

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public ChargeBand GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand)
        {
            if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return ChargeBand.E;
            }
            else
            {
                if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && vatRegistered
                    && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.A;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.B;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
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
