namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using Xml.Schemas;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class AnnualTurnoverMismatchTests
    {
        [Theory]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1F)]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000000F)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 10000001F)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1000000000000F)]
        public void AnnualTurnoverMatchesWithBand_ShouldReturnValidResult(annualTurnoverBandType annualTurnoverBand, float annualTurnover)
        {
            var producer = new producerType
            {
                annualTurnover = annualTurnover,
                annualTurnoverBand = annualTurnoverBand
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1F)]
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1000000F)] // Edge case
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000001F)] // Edge case
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000000000000F)]
        public void AnnualTurnoverDoesNotMatchWithBand_ShouldReturnInvalidResult_WithProducerName_AndProducerRegistrationNumber_AndWarningStatus(annualTurnoverBandType annualTurnoverBand, float annualTurnover)
        {
            const string name = "Some company";
            const string prn = "ABC12345";

            var producer = new producerType
            {
                annualTurnover = annualTurnover,
                annualTurnoverBand = annualTurnoverBand,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = name
                    }
                },
                registrationNo = prn
            };

            var result = Rule().Evaluate(producer);

            Assert.False(result.IsValid);
            Assert.Contains(name, result.Message);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        private AnnualTurnoverMismatch Rule()
        {
            return new AnnualTurnoverMismatch();
        }
    }
}
