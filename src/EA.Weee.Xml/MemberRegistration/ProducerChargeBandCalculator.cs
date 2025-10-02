namespace EA.Weee.Xml.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using System;
    using System.Threading.Tasks;

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
            var producerCountry = producer.GetProducerCountry();
            var complianceYear = int.Parse(scheme.complianceYear);
            var competentAuthority = ConvertToCompetentAuthorityType(producerCountry);
            var annualTurnoverBand = ConvertToAnnualTurnoverBand(producer.annualTurnoverBand);
            var eeePlacedOnMarketBand = ConvertToEEEPlacedOnMarketBand(producer.eeePlacedOnMarketBand);
            var asOfUtc = DateTime.UtcNow;

            // For producers based in England or outside the UK, the annual turnover band is not applicable
            if (producerCountry == countryType.UKENGLAND || !IsUKCountry(producerCountry))
            {
                annualTurnoverBand = AnnualTurnoverBand.NotApplicable;
            }

            // Use the enhanced data-driven method to get the complete charge band amount record
            var chargeBandAmount = await fetchProducerCharge.GetChargeBandAmountAsync(
                competentAuthority,
                producer.VATRegistered,
                annualTurnoverBand,
                eeePlacedOnMarketBand,
                complianceYear,
                asOfUtc);

            return new ProducerCharge()
            {
                ChargeBandAmount = chargeBandAmount,
                Amount = chargeBandAmount.Amount
            };
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

        /// <summary>
        /// Determines if the country is a UK country
        /// </summary>
        private bool IsUKCountry(countryType countryType)
        {
            return countryType == countryType.UKENGLAND ||
                   countryType == countryType.UKSCOTLAND ||
                   countryType == countryType.UKWALES ||
                   countryType == countryType.UKNORTHERNIRELAND;
        }

        private CompetentAuthorityType ConvertToCompetentAuthorityType(countryType countryType)
        {
            switch (countryType)
            {
                case countryType.UKENGLAND:
                    return CompetentAuthorityType.England;
                case countryType.UKWALES:
                    return CompetentAuthorityType.Wales;
                case countryType.UKSCOTLAND:
                    return CompetentAuthorityType.Scotland;
                case countryType.UKNORTHERNIRELAND:
                    return CompetentAuthorityType.NorthernIreland;
                default:
                    return CompetentAuthorityType.NonUK;
            }
        }

        private AnnualTurnoverBand ConvertToAnnualTurnoverBand(annualTurnoverBandType annualTurnoverBandType)
        {
            switch (annualTurnoverBandType)
            {
                case annualTurnoverBandType.Greaterthanonemillionpounds:
                    return AnnualTurnoverBand.Greaterthanonemillionpounds;
                case annualTurnoverBandType.Lessthanorequaltoonemillionpounds:
                    return AnnualTurnoverBand.Lessthanorequaltoonemillionpounds;
                default:
                    return AnnualTurnoverBand.Lessthanorequaltoonemillionpounds;
            }
        }

        private EEEPlacedOnMarketBand ConvertToEEEPlacedOnMarketBand(eeePlacedOnMarketBandType eeePlacedOnMarketBandType)
        {
            switch (eeePlacedOnMarketBandType)
            {
                case eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket:
                    return EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                case eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket:
                    return EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket;
                default:
                    return EEEPlacedOnMarketBand.Lessthan5TEEEplacedonmarket;
            }
        }
    }
}
