namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Scheme;
    using Serilog;
    using Xml.MemberRegistration;

    public class MigrationEnvironmentAgencyProducerChargeBandCalculator : IMigrationEnvironmentAgencyProducerChargeBandCalculator, IMigrationChargeBandCalculator
    {
        private readonly IMigrationFetchProducerCharge fetchProducerCharge;
        private readonly IMigrationRegisteredProducerDataAccess registeredProducerDataAccess;

        public MigrationEnvironmentAgencyProducerChargeBandCalculator(IMigrationFetchProducerCharge fetchProducerCharge, IMigrationRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.fetchProducerCharge = fetchProducerCharge;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public ProducerCharge GetProducerChargeBand(schemeType scheme, producerType producer, MemberUpload memberUpload)
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

            return fetchProducerCharge.GetCharge(band);
        }

        public bool IsMatch(schemeType scheme, producerType producer, MemberUpload upload, string name)
        {
            var year = int.Parse(scheme.complianceYear);
            var previousProducerSubmission = registeredProducerDataAccess.GetProducerRegistrationForInsert(producer.registrationNo, year, scheme.approvalNo, upload, name, producer);

            if (year > 2018)
            {
                if (producer.status == statusType.I)
                {
                    Log.Information(string.Format("calc {0}", name));
                    return true;
                }
                if (producer.status == statusType.A && previousProducerSubmission == null)
                {
                    Log.Information(string.Format("calc {0}", name));
                    return true;
                }
            }
            return false;
        }
    }
}
