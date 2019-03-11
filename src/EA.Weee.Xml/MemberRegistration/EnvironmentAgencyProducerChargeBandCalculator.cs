namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class EnvironmentAgencyProducerChargeBandCalculator : IEnvironmentAgencyProducerChargeBandCalculator
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
                producerType.VATRegistered && producerCountry == countryType.UKENGLAND)
                {
                    return ChargeBand.A2;
                }
                else if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                producerType.VATRegistered && 
                (producerType.authorisedRepresentative.overseasProducer == null ? false : true))
                {
                    return ChargeBand.D3;
                }
                else if (producerType.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                producerCountry == countryType.UKENGLAND && 
                !producerType.VATRegistered)
                {
                    return ChargeBand.C2;
                }
                else
                { 
                    return ChargeBand.D2;
                }
            }
        }
    }
}
