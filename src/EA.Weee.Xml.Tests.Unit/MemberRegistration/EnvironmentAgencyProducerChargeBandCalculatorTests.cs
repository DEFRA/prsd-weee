namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;

    public class EnvironmentAgencyProducerChargeBandCalculatorTests
    {
        [Fact]
        public void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA2()
        {
            countryType country = countryType.UKENGLAND;
            producerType producer = SetUpProducer(country);

            // Act
            ChargeBand result = new EnvironmentAgencyProducerChargeBandCalculator().GetProducerChargeBand(producer);

            // Assert
            Assert.Equal(ChargeBand.A2, result);
        }

        private static producerType SetUpProducer(countryType countryType)
        {
            countryType = new countryType();
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
                annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds,
                VATRegistered = true,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket,
                producerBusiness = producerBusiness
            };
            return producer;
        }
    }
}
