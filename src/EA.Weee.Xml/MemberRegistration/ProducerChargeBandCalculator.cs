namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;    

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public ChargeBand GetProducerChargeBand(producerType producerType)
        {
            var producerCountry = producerType.GetProducerCountry();

            if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return ChargeBand.E;
            }
            else
            {
                if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                    producerType.VATRegistered && 
                    (producerCountry == countryType.UKSCOTLAND ||
                    producerCountry == countryType.UKWALES ||
                    producerCountry == countryType.UKNORTHERNIRELAND) &&
                    producerType.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
                {
                    return ChargeBand.A;
                }
                else if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                         producerType.VATRegistered && 
                        (producerCountry == countryType.UKSCOTLAND ||
                         producerCountry == countryType.UKWALES ||
                         producerCountry == countryType.UKNORTHERNIRELAND) && 
                         producerType.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds)
                {
                    return ChargeBand.B;
                }
                else if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                        !producerType.VATRegistered && 
                        (producerCountry == countryType.UKSCOTLAND ||
                          producerCountry == countryType.UKWALES ||
                          producerCountry == countryType.UKNORTHERNIRELAND) &&
                          producerType.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
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
