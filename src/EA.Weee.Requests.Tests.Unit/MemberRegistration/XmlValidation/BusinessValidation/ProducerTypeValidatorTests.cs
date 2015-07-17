namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Linq;
    using Domain;
    using FluentValidation;
    using FluentValidation.Internal;
    using RequestHandlers;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation;
    using Xunit;

    public class ProducerTypeValidatorTests
    {
        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Amendment_RegistrationNumberIsNullOrEmpty_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError(string registrationNumber, string tradingName)
        {
            var validationResult = new ProducerTypeValidator().Validate(new producerType
            {
                tradingName = tradingName, 
                status = statusType.A, 
                registrationNo = registrationNumber
            }, "registrationNo");

            Assert.False(validationResult.IsValid);
            Assert.Contains(tradingName, validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Fact]
        public void Amendment_RegistrationNumberIsNotNullOrEmpty_PassesValidation()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var validationResult = new ProducerTypeValidator().Validate(new producerType
            {
                tradingName = validTradingName, 
                status = statusType.A, 
                registrationNo = validRegistrationNumber
            }, new RulesetValidatorSelector("registrationNo"));

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Insert_RegistrationNumberIsNotNullOrEmpty_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var validationResult = new ProducerTypeValidator().Validate(new producerType
            {
                tradingName = validTradingName, 
                status = statusType.I, 
                registrationNo = validRegistrationNumber
            }, new RulesetValidatorSelector("registrationNo"));

            Assert.False(validationResult.IsValid);
            Assert.Contains(validTradingName, validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }

        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Insert_RegistrationNumberIsNullOrEmpty_PassesValidation(string registrationNumber, string tradingName)
        {
            var validationResult = new ProducerTypeValidator().Validate(new producerType
            {
                tradingName = tradingName, 
                status = statusType.I, 
                registrationNo = registrationNumber
            }, new RulesetValidatorSelector("registrationNo"));

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(countryType.UKENGLAND)]
        [InlineData(countryType.UKNORTHERNIRELAND)]
        [InlineData(countryType.UKSCOTLAND)]
        [InlineData(countryType.UKWALES)]
        public void OfficeCountryIsInUnitedKingdom_PassesValidation(countryType someUkCountry)
        {
            var producer = new producerType
            {
                producerBusiness =
                    new producerBusinessType
                    {
                        Item =
                            new partnershipType
                            {
                                principalPlaceOfBusiness =
                                    new contactDetailsContainerType
                                    {
                                        contactDetails =
                                            new contactDetailsType
                                            {
                                                address = new addressType { country = someUkCountry }
                                            }
                                    }
                            }
                    }
            };

            var validationResult = new ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector("authorisedRepresentative"));

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(countryType.TURKEY)]
        public void OfficeCountryIsNotInUnitedKingdom_FailsValidation_AndIncludesTradingNameInMessage_AndErrorLevelIsError(countryType someNonUkCountry)
        {
            const string validTradingName = "MyCompany";

            var producer = new producerType
            {
                tradingName = validTradingName,
                producerBusiness =
                    new producerBusinessType
                    {
                        Item =
                            new partnershipType
                            {
                                principalPlaceOfBusiness =
                                    new contactDetailsContainerType
                                    {
                                        contactDetails =
                                            new contactDetailsType
                                            {
                                                address = new addressType { country = someNonUkCountry }
                                            }
                                    }
                            }
                    }
            };

            var validationResult = new ProducerTypeValidator().Validate(producer, new RulesetValidatorSelector("authorisedRepresentative"));

            Assert.False(validationResult.IsValid);
            Assert.Contains(validTradingName, validationResult.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, validationResult.Errors.Single().CustomState);
        }
    }
}
