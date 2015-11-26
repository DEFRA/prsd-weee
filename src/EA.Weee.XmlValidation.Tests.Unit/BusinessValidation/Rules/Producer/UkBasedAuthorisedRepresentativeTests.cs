namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using System;
    using Core.Shared;
    using Xml;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class UkBasedAuthorisedRepresentativeTests
    {
        [Theory]
        [InlineData(countryType.UKENGLAND)]
        [InlineData(countryType.UKNORTHERNIRELAND)]
        [InlineData(countryType.UKSCOTLAND)]
        [InlineData(countryType.UKWALES)]
        public void AuthorisedRepresentativeOfficeCountryIsInUnitedKingdom_PassesValidation(countryType someUkCountry)
        {
            var producer = new producerType
            {
                authorisedRepresentative = new authorisedRepresentativeType
                {
                    overseasProducer = new overseasProducerType()
                },
                producerBusiness = MakeProducerBusinessTypeInCountry(someUkCountry)
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void AuthorisedRepresentativeOfficeCountryIsNotInUnitedKingdom_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string validTradingName = "MyCompany";

            const countryType someNonUkCountry = countryType.TURKEY;

            var producer = new producerType
            {
                tradingName = validTradingName,
                authorisedRepresentative = new authorisedRepresentativeType
                {
                    overseasProducer = new overseasProducerType()
                },
                producerBusiness = MakeProducerBusinessTypeInCountry(someNonUkCountry)
            };

            var result = Rule().Evaluate(producer);

            Assert.False(result.IsValid);
            Assert.Contains(producer.GetProducerName(), result.Message);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }

        [Fact]
        public void NotAnAuthorisedRepresentativeButIsInNonUkCountry_PassesValidation()
        {
            const countryType SomeNonUkCountry = countryType.TURKEY;

            var producer = new producerType
            {
                authorisedRepresentative = new authorisedRepresentativeType { overseasProducer = null },
                producerBusiness = MakeProducerBusinessTypeInCountry(SomeNonUkCountry)
            };

            var validationResult = Rule().Evaluate(producer);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void AuthorisedRepresentativeIsNotAValidBusinessType_ThrowsArgumentException()
        {
            var producer = new producerType
            {
                authorisedRepresentative =
                    new authorisedRepresentativeType { overseasProducer = new overseasProducerType() },
                producerBusiness = new producerBusinessType { Item = new object() }
            };

            Assert.Throws<ArgumentException>(() => Rule().Evaluate(producer));
        }

        private UkBasedAuthorisedRepresentative Rule()
        {
            return new UkBasedAuthorisedRepresentative();
        }

        private producerBusinessType MakeProducerBusinessTypeInCountry(countryType country)
        {
            return new producerBusinessType
            {
                Item =
                    new partnershipType
                    {
                        principalPlaceOfBusiness =
                            new contactDetailsContainerType
                            {
                                contactDetails =
                                    new contactDetailsType { address = new addressType { country = country } }
                            }
                    }
            };
        }
    }
}
