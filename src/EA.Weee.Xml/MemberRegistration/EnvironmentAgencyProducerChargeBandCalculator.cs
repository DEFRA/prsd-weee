namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class EnvironmentAgencyProducerChargeBandCalculator : IEnvironmentAgencyProducerChargeBandCalculator, IProducerChargeBandCalculator
    {
        public async Task<ChargeBand?> GetProducerChargeBand(schemeType scheme, producerType producer)
        {
            var producerCountry = producer.GetProducerCountry();

            if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return await Task.FromResult(ChargeBand.E);
            }
            else
            { 
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                    producer.VATRegistered && producerCountry == countryType.UKENGLAND)
                {
                    return await Task.FromResult(ChargeBand.A2);
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         producer.VATRegistered && 
                (producerCountry != countryType.UKENGLAND && 
                 producerCountry != countryType.UKSCOTLAND &&
                 producerCountry != countryType.UKWALES &&
                 producerCountry != countryType.UKNORTHERNIRELAND))
                {
                    return await Task.FromResult(ChargeBand.D3);
                }
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                    producer.VATRegistered &&
                    (producerCountry == countryType.UKSCOTLAND ||
                    producerCountry == countryType.UKWALES ||
                    producerCountry == countryType.UKNORTHERNIRELAND) &&
                    producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
                {
                    return await Task.FromResult(ChargeBand.A);
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         producer.VATRegistered &&
                        (producerCountry == countryType.UKSCOTLAND ||
                         producerCountry == countryType.UKWALES ||
                         producerCountry == countryType.UKNORTHERNIRELAND) &&
                         producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds)
                {
                    return await Task.FromResult(ChargeBand.B);
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                producerCountry == countryType.UKENGLAND && 
                !producer.VATRegistered)
                {
                    return await Task.FromResult(ChargeBand.C2);
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                !producer.VATRegistered &&
                (producerCountry != countryType.UKENGLAND && 
                 producerCountry != countryType.UKSCOTLAND &&
                 producerCountry != countryType.UKWALES &&
                 producerCountry != countryType.UKNORTHERNIRELAND))
                {
                    return await Task.FromResult(ChargeBand.D2);
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         !producer.VATRegistered &&
                        (producerCountry == countryType.UKSCOTLAND ||
                         producerCountry == countryType.UKWALES ||
                         producerCountry == countryType.UKNORTHERNIRELAND) &&
                         producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
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

            return year > 2018 && producer.status == statusType.I ? true : false;
        }
    }
}
