namespace EA.Weee.Xml.MemberRegistration
{
    using System;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using EA.Weee.DataAccess.DataAccess;
    using Xml.MemberRegistration;

    public class EnvironmentAgencyProducerChargeBandCalculator : IEnvironmentAgencyProducerChargeBandCalculator, IProducerChargeBandCalculator
    {
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public EnvironmentAgencyProducerChargeBandCalculator(IFetchProducerCharge fetchProducerCharge, IRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.fetchProducerCharge = fetchProducerCharge;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public async Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer)
        {
            var producerCountry = producer.GetProducerCountry();
            ChargeBand band;

            if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                band = ChargeBand.E;
            }
            else
            { 
                if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                    producer.VATRegistered && producerCountry == countryType.UKENGLAND)
                {
                    band = ChargeBand.A2;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         producer.VATRegistered && 
                (producerCountry != countryType.UKENGLAND && 
                 producerCountry != countryType.UKSCOTLAND &&
                 producerCountry != countryType.UKWALES &&
                 producerCountry != countryType.UKNORTHERNIRELAND))
                {
                    band = ChargeBand.D3;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                    producer.VATRegistered &&
                    (producerCountry == countryType.UKSCOTLAND ||
                    producerCountry == countryType.UKWALES ||
                    producerCountry == countryType.UKNORTHERNIRELAND) &&
                    producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
                {
                    band = ChargeBand.A;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         producer.VATRegistered &&
                        (producerCountry == countryType.UKSCOTLAND ||
                         producerCountry == countryType.UKWALES ||
                         producerCountry == countryType.UKNORTHERNIRELAND) &&
                         producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds)
                {
                    band = ChargeBand.B;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket && 
                producerCountry == countryType.UKENGLAND && 
                !producer.VATRegistered)
                {
                    band = ChargeBand.C2;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                !producer.VATRegistered &&
                (producerCountry != countryType.UKENGLAND && 
                 producerCountry != countryType.UKSCOTLAND &&
                 producerCountry != countryType.UKWALES &&
                 producerCountry != countryType.UKNORTHERNIRELAND))
                {
                    band = ChargeBand.D2;
                }
                else if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket &&
                         !producer.VATRegistered &&
                        (producerCountry == countryType.UKSCOTLAND ||
                         producerCountry == countryType.UKWALES ||
                         producerCountry == countryType.UKNORTHERNIRELAND) &&
                         producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
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
            var previousProducerSubmission = registeredProducerDataAccess.GetProducerRegistration(producer.registrationNo, year, scheme.approvalNo);
           
            return year > 2018 && producer.status == statusType.I || producer.status == statusType.A && (previousProducerSubmission != null) ? true : false;
        }
    }
}
