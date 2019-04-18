namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using Xml.MemberRegistration;
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

        [Fact]
        public void GetProducerCountry_ProducerIsCompanyType_ReturnsRegisteredOfficeCountry()
        {
            var producer = SetRegisteredOfficeOrPPoBDetails(false);

            var producerCountry = producer.GetProducerCountry();
            Assert.Equal("UKENGLAND", producerCountry.ToString());
        }

        [Fact]
        public void GetProducerCountry_ProducerIsBusinessPartnerShipType_ReturnsPPOBCountry()
        {
            var producer = SetRegisteredOfficeOrPPoBDetails(true);

            var producerCountry = producer.GetProducerCountry();
            Assert.Equal("UKENGLAND", producerCountry.ToString());
        }

        private static producerType SetRegisteredOfficeOrPPoBDetails(bool hasPartnership)
        {
            producerBusinessType producerBusiness = null;

            var contactDetailsInfo = new contactDetailsContainerType()
            {
                contactDetails = new contactDetailsType()
                {
                    address = new addressType()
                    {
                        country = countryType.UKENGLAND
                    }
                }
            };

            if (hasPartnership)
            {
                var producerPartnership = new partnershipType()
                {
                    partnershipName = "MiddleEarth",
                    principalPlaceOfBusiness = contactDetailsInfo
                };

                producerBusiness = new producerBusinessType()
                {
                    Item = producerPartnership
                };
            }
            else
            {
                var producerCompany = new companyType()
                {
                    companyName = "Rivendell",
                    registeredOffice = contactDetailsInfo
                };

                producerBusiness = new producerBusinessType()
                {
                    Item = producerCompany
                };
            }

            var producer = new producerType
            {
                producerBusiness = producerBusiness
            };

            return producer;
        }
    }
}