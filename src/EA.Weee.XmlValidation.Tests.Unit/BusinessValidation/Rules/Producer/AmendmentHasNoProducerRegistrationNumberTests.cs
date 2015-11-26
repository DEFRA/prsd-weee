namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using Core.Shared;
    using Xml;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class AmendmentHasNoProducerRegistrationNumberTests
    {
        [Theory]
        [InlineData(null, "TestCompany")]
        [InlineData("", "TestCompany")]
        public void Amendment_RegistrationNumberIsNullOrEmpty_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError(string registrationNumber, string tradingName)
        {
            var producer = new producerType
            {
                tradingName = tradingName,
                status = statusType.A,
                registrationNo = registrationNumber
            };

            var result = Rule().Evaluate(producer);

            Assert.False(result.IsValid);
            Assert.Contains(producer.GetProducerName(), result.Message);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }

        [Fact]
        public void Amendment_RegistrationNumberIsProdived_PassesValidation()
        {
            var producer = new producerType
            {
                tradingName = "TestCompany",
                status = statusType.A,
                registrationNo = "ABC12345"
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        private AmendmentHasNoProducerRegistrationNumber Rule()
        {
            return new AmendmentHasNoProducerRegistrationNumber();
        }
    }
}
