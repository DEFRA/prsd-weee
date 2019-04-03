namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using EA.Weee.Xml.MemberRegistration;
    using FakeItEasy;
    using Xunit;

    public class ProducerChargeBandCalculatorTests
    {
        private readonly ProducerChargeBandCalculator producerChargeBandCalculator;
        private readonly IFetchProducerCharge fetchProducerCharge;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public ProducerChargeBandCalculatorTests()
        {
            fetchProducerCharge = A.Fake<IFetchProducerCharge>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();

            producerChargeBandCalculator = new ProducerChargeBandCalculator(fetchProducerCharge, registeredProducerDataAccess);
        }

        /// <summary>
        /// This test ensures that the charge band is A when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered_ProducerChargeBandAShouldBeRetrieved()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds,
                VATRegistered = true,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds,
                VATRegistered = true,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };
            var producerCharge = new ProducerCharge();

            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.A)).Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        /// <summary>
        /// This test ensures that the charge band is B when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is VAT registered.
        /// </summary>
        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered_ProducerChargeBandBShouldBeRetrieved()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = true,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.B)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_VATRegistered__ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = true,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };
            var producerCharge = new ProducerCharge();

            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.B)).Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        /// <summary>
        /// This test ensures that the charge band is C when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is at most £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ProducerChargeBandCShouldBeRetrieved()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_Lessthanorequaltoonemillionpounds_NotVATRegistered_ProducerChargeForChargeBandShouldBeReturned()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };
            var producerCharge = new ProducerCharge();

            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.C)).Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        /// <summary>
        /// This test ensures that the charge band is D when the amount of EEE placed on the market per year
        /// is at least 5 Tonnes, the annual turnover is greater than £1,000,000 and the company is not VAT registered.
        /// </summary>
        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered_ProducerChargeBandDShouldBeRetrieved()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_Morethanorequalto5TEEEplacedonmarket_GreaterthanonemillionpoundsTurnover_NotVATRegistered__ProducerChargeForChargeBandDShouldBeReturned()
        {
            // Arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Greaterthanonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket
            };
            var producerCharge = new ProducerCharge();

            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.D)).Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        /// <summary>
        /// This test ensures that the charge band is E when the amount of EEE placed on the market per year
        /// is less than 5 Tonnes.
        /// </summary>
        [Fact]
        public async void GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ProducerChargeBandEShouldBeRetrieved()
        {
            //arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket
            };

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.E)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetProducerChargeBand_Lessthan5TEEEplacedonmarket_ProducerChargeForChargeBandShouldBeReturned()
        {
            //arrange
            var producer = new producerType
            {
                annualTurnoverBand = annualTurnoverBandType.Lessthanorequaltoonemillionpounds,
                VATRegistered = false,
                eeePlacedOnMarketBand = eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket
            };
            var producerCharge = new ProducerCharge();

            A.CallTo(() => fetchProducerCharge.GetCharge(ChargeBand.E)).Returns(producerCharge);

            // Act
            var result = await producerChargeBandCalculator.GetProducerChargeBand(A.Dummy<schemeType>(), producer);

            // Assert
            Assert.Equal(producerCharge, result);
        }

        [Theory]
        [InlineData("2018")]
        [InlineData("2017")]
        [InlineData("2016")]
        public void IsMatch_GivenSchemeIs2018OrBeforeAndProducerInsert_TrueShouldBeReturned(string year)
        {
            var scheme = new schemeType() {complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.True(result);
        }

        [Theory]
        [InlineData("2019")]
        [InlineData("2020")]
        [InlineData("2021")]
        public void IsMatch_GivenSchemeIsPost2018_FalseShouldBeReturned(string year)
        {
            var scheme = new schemeType() { complianceYear = year };
            var producer = new producerType { status = statusType.I };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_GivenProducerIsAmendment_FalseShouldBeReturned()
        {
            var scheme = new schemeType() { complianceYear = "2019" };
            var producer = new producerType { status = statusType.A };

            var result = producerChargeBandCalculator.IsMatch(scheme, producer);

            Assert.False(result);
        }
    }
}
