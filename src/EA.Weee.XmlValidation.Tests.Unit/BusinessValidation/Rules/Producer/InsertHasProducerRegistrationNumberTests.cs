namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using Core.Shared;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class InsertHasProducerRegistrationNumberTests
    {
        [Fact]
        public void Insert_RegistrationNumberIsNotNullOrEmpty_FailsValidation_AndIncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string validRegistrationNumber = "ABC12345";
            const string validTradingName = "MyCompany";

            var producer = new producerType
            {
                tradingName = validTradingName,
                status = statusType.I,
                registrationNo = validRegistrationNumber
            };

            var result = Rule().Evaluate(producer);

            Assert.False(result.IsValid);
            Assert.Contains(producer.GetProducerName(), result.Message);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }

         [Theory]
         [InlineData(null, "TestCompany")]
         [InlineData("", "TestCompany")]
         public void Insert_RegistrationNumberIsNullOrEmpty_PassesValidation(string registrationNumber, string tradingName)
         {
             var producer = new producerType
             {
                 tradingName = tradingName,
                 status = statusType.I,
                 registrationNo = registrationNumber
             };

             var validationResult = Rule().Evaluate(producer);

             Assert.True(validationResult.IsValid);
         }

        private InsertHasProducerRegistrationNumber Rule()
        {
            return new InsertHasProducerRegistrationNumber();
        }
    }
}
