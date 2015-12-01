namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using System.Globalization;
    using Xml.Schemas;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class AnnualTurnoverMismatchTests
    {
        [Theory]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1)]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000000)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 10000001)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1000000000000)]
        public void AnnualTurnoverMatchesWithBand_ShouldReturnValidResult(annualTurnoverBandType annualTurnoverBand, double annualTurnover)
        {
            var producer = new producerType
            {
                annualTurnover = decimal.Parse(annualTurnover.ToString(CultureInfo.InvariantCulture)),
                annualTurnoverBand = annualTurnoverBand
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1)]
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1000000)] // Edge case
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000001)] // Edge case
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000000000000)]
        public void AnnualTurnoverDoesNotMatchWithBand_ShouldReturnInvalidResult_WithProducerName_AndProducerRegistrationNumber_AndWarningStatus(annualTurnoverBandType annualTurnoverBand, double annualTurnover)
        {
            const string name = "Some company";
            const string prn = "ABC12345";

            var producer = new producerType
            {
                annualTurnover = decimal.Parse(annualTurnover.ToString(CultureInfo.InvariantCulture)),
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
