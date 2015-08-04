namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation
{
    using RequestHandlers;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.Extensions;
    using Xunit;

    public class DeserializedXmlExtensionsTests
    {
        [Fact]
        public void GetProducerName_ProducerBusinessIsNull_ReturnsTradingName()
        {
            const string TradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = TradingName
            };

            Assert.Equal(TradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerBusinessItemIsNull_ReturnsTradingName()
        {
            const string TradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = TradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = null
                }
            };

            Assert.Equal(TradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsCompanyTypeAndNameExists_ReturnsCompanyName()
        {
            const string CompanyName = "some company name";

            var producer = new producerType
            {
                tradingName = "some trading name",
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = CompanyName
                    }
                }
            };

            Assert.Equal(CompanyName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsCompanyTypeButNameIsNull_ReturnsTradingName()
        {
            const string TradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = TradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = null
                    }
                }
            };

            Assert.Equal(TradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsPartnershipTypeAndNameExists_ReturnsPartnershipName()
        {
            const string PartnershipName = "some company name";

            var producer = new producerType
            {
                tradingName = "some trading name",
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = PartnershipName
                    }
                }
            };

            Assert.Equal(PartnershipName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsPartnershipTypeButNameIsNull_ReturnsTradingName()
        {
            const string TradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = TradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = null
                    }
                }
            };

            Assert.Equal(TradingName, producer.GetProducerName());
        }
    }
}
