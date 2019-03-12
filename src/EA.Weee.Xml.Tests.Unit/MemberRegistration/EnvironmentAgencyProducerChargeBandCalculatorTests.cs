namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;

    public class EnvironmentAgencyProducerChargeBandCalculatorTests
    {
        [Fact]
        public void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA()
        {
            countryType country = countryType.UKENGLAND;
            eeePlacedOnMarketBandType eeePlacedOnMarketBandType = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
            annualTurnoverBandType annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            producerType producer = SetUpProducer(country, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            ChargeBand result = new EnvironmentAgencyProducerChargeBandCalculator().GetProducerChargeBand(producer);

            // Assert
            Assert.Equal(ChargeBand.A, result);
        }

        public void GetProducerChargeBand_NonUKEngland_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered_ReturnsChargeBandD2()
        {
            countryType country = countryType.FRANCE;
            eeePlacedOnMarketBandType eeePlacedOnMarketBandType = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket;
            annualTurnoverBandType annualTurnoverBandType = 0;
            producerType producer = SetUpProducer(country, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand result = new EnvironmentAgencyProducerChargeBandCalculator().GetProducerChargeBand(producer);

            // Assert
            Assert.Equal(ChargeBand.D2, result);
        }
        private static producerType SetUpProducer(countryType countryType, eeePlacedOnMarketBandType eeePlacedOnMarketBandType, annualTurnoverBandType annualTurnoverBandType, bool vatRegistered)
        {
            countryType = new countryType();
            eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            annualTurnoverBandType = new annualTurnoverBandType();

            var producerCountry = new companyType()
            {
                companyName = "Test company",
                companyNumber = "Test CRN",
                registeredOffice = new contactDetailsContainerType()
                {
                    contactDetails = new contactDetailsType()
                    {
                        address = new addressType()
                        {
                            country = countryType
                        }
                    }
                }
            };

            var producerBusiness = new producerBusinessType()
            {
                Item = producerCountry
            };

            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType,
                VATRegistered = vatRegistered,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType,
                producerBusiness = producerBusiness
            };
            return producer;
        }
    }
}
