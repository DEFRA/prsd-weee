namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;

    public class EnvironmentAgencyProducerChargeBandCalculatorTests
    {
        private readonly EnvironmentAgencyProducerChargeBandCalculator environmentAgencyProducerChargeBandCalculator;
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public EnvironmentAgencyProducerChargeBandCalculatorTests()
        {
            fetchProducerCharge = A.Fake<IFetchProducerCharge>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();

            environmentAgencyProducerChargeBandCalculator = new EnvironmentAgencyProducerChargeBandCalculator(fetchProducerCharge, registeredProducerDataAccess);
        }

        [Fact]
        public async void GetProducerChargeBand_Lessthanorequalto5TEEEplacedonmarket_ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            eeePlacedOnMarketBandType = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.E)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_Lessthanorequalto5TEEEplacedonmarket_ChargeBandEShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            eeePlacedOnMarketBandType = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.E)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A2)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ChargeBandA2ShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.FRANCE;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D3)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_VATRegistered_ChargeBandD3ShouldBeRetrieved()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.FRANCE;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D3)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKWales_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.UKWALES;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKWales_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ChargeBandAShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKWALES;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.UKSCOTLAND;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.B)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_VATRegistered_ChargeBandBShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKSCOTLAND;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, true);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.B)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C2)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKEngland_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered_ChargeBandC2ShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.FRANCE;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D2)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_NonUKCountry_Morethanorequalto5TEEEplacedonmarket_NotVATRegistered_ChargeBandD2ShouldBeRetrieved()
        {
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.FRANCE;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKNorthernIreland_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.UKNORTHERNIRELAND;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKNorthernIreland_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ChargeBandDShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKNORTHERNIRELAND;
            annualTurnoverBandType = annualTurnoverBandType.Greaterthanonemillionpounds;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_NotVATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();
            var producerCharge = new ProducerCharge();

            countryType = countryType.UKSCOTLAND;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C)).Returns(producerCharge);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Fact]
        public async void GetProducerChargeBand_UKScotland_Morethanorequalto5TEEEplacedonmarket_LessthanonemillionpoundsTurnover_NotVATRegistered_ChargeBandCShouldBeRetrieved()
        {
            //Arrange
            var countryType = new countryType();
            var eeePlacedOnMarketBandType = new eeePlacedOnMarketBandType();
            var annualTurnoverBandType = new annualTurnoverBandType();

            countryType = countryType.UKSCOTLAND;
            var producer = SetUpProducer(countryType, eeePlacedOnMarketBandType, annualTurnoverBandType, false);

            // Act
            var result = await environmentAgencyProducerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C)).MustHaveHappened(Repeated.Exactly.Once);
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

        [Fact]
        public void IsMatch_GivenPreviousSubmission_TrueShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2020" };
            var producerType = new producerType { status = statusType.A };
            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(A<string>._, A<int>._, A<string>._)).Returns((RegisteredProducer)null);

            var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producerType);

            Assert.True(result);
        }

        //[Fact]
        //public void IsMatch_Insert_TrueShouldBeReturned()
        //{
        //    var scheme = new schemeType() { complianceYear = "2019" };
        //    var producerType = new producerType { status = statusType.I };

        //    var result = environmentAgencyProducerChargeBandCalculator.IsMatch(scheme, producerType);

        //    Assert.True(result);
        //}

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