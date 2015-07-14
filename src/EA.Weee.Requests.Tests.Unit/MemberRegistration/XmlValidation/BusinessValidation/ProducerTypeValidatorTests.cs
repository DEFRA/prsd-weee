namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Linq;
    using Domain;
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
            });

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
            });

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
            });

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
            });

            Assert.True(validationResult.IsValid);
        }
    }
}
