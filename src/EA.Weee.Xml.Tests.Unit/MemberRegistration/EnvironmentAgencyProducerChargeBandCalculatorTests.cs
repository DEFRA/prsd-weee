namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;

    public class EnvironmentAgencyProducerChargeBandCalculatorTests
    {
        private readonly EnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;

        public EnvironmentAgencyProducerChargeBandCalculatorTests()
        {
            environmentAgencyProducerChargeBandCalculator = new EnvironmentAgencyProducerChargeBandCalculator();
        }

        [Fact]
        public async void GetProducerChargeBand_Lessthanorequalto5TEEEplacedonmarket_ReturnsChargeBandE()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            eeePlacedOnMarketBandType = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.E, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ReturnsChargeBandA2()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.A2, result);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ReturnsChargeBandD3()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.FRANCE;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.D3, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKWales_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandA()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKWALES;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.A, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_VATRegistered_ReturnsChargeBandB()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKSCOTLAND;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.B, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered_ReturnsChargeBandC2()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.C2, result);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered_ReturnsChargeBandD2()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.FRANCE;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.D2, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKNorthernIreland_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ReturnsChargeBandD()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKNORTHERNIRELAND;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.D, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_NotVATRegistered_ReturnsChargeBandC()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKSCOTLAND;
            producerType producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            ChargeBand? result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(ChargeBand.C, result);
        }

        [Theory]
        [InlineData("2018")]
        [InlineData("2017")]
        [InlineData("2016")]
        public void IsMatch_GivenSchemeIs2018OrBeforeAndProducerInsert_FalseShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        [InlineData("2021")]
        public void IsMatch_GivenSchemeIsPost2018_TrueShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.True(result);
        }

        [Fact]
        public void IsMatch_GivenProducerIsAmendment_FalseShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2019" };
            var producer = new producerType { status = statusType.A };

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        private static producerType SetUpProducer(countryType countryType, eeePlacedOnMarketBandType eeePlacedOnMarketBandType, annualTurnoverBandType annualTurnoverBandType, bool vatRegistered)
        {
            var producerCompany = new companyType()
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
                Item = producerCompany
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
