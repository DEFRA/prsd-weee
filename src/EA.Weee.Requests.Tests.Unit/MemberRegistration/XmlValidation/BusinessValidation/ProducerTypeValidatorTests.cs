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
    }
}
