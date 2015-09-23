namespace EA.Weee.Xml.Tests.Unit
{
    using Schemas;
    using Xunit;

    public class ExtensionsTests
    {
        [Fact]
        public void GetProducerName_ProducerBusinessIsNull_ReturnsTradingName()
        {
            const string tradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = tradingName
            };

            Assert.Equal(tradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerBusinessItemIsNull_ReturnsTradingName()
        {
            const string tradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = tradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = null
                }
            };

            Assert.Equal(tradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsCompanyTypeAndNameExists_ReturnsCompanyName()
        {
            const string companyName = "some company name";

            var producer = new producerType
            {
                tradingName = "some trading name",
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = companyName
                    }
                }
            };

            Assert.Equal(companyName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsCompanyTypeButNameIsNull_ReturnsTradingName()
        {
            const string tradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = tradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = null
                    }
                }
            };

            Assert.Equal(tradingName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsPartnershipTypeAndNameExists_ReturnsPartnershipName()
        {
            const string partnershipName = "some company name";

            var producer = new producerType
            {
                tradingName = "some trading name",
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = partnershipName
                    }
                }
            };

            Assert.Equal(partnershipName, producer.GetProducerName());
        }

        [Fact]
        public void GetProducerName_ProducerIsPartnershipTypeButNameIsNull_ReturnsTradingName()
        {
            const string tradingName = "some trading name";

            var producer = new producerType
            {
                tradingName = tradingName,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = null
                    }
                }
            };

            Assert.Equal(tradingName, producer.GetProducerName());
        }
    }
}