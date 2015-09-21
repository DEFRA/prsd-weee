namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;
    using Xunit;

    public class AnnualTurnoverMismatchEvaluatorTests
    {
        [Theory]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1F)]
        [InlineData(annualTurnoverBandType.Lessthanorequaltoonemillionpounds, 1000000F)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 10000001F)] // Edge case
        [InlineData(annualTurnoverBandType.Greaterthanonemillionpounds, 1000000000000F)]
        public void AnnualTurnoverMatchesWithBand_ShouldReturnValidResult(annualTurnoverBandType annualTurnoverBand, float annualTurnover)
        {
            var rule = new AnnualTurnoverMismatch(new producerType
            {
                annualTurnover = annualTurnover,
                annualTurnoverBand = annualTurnoverBand
            });

            var result = Evaluator().Evaluate(rule);

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

            var rule = new AnnualTurnoverMismatch(new producerType
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
            });

            var result = Evaluator().Evaluate(rule);

            Assert.False(result.IsValid);
            Assert.Contains(name, result.Message);
            Assert.Contains(prn, result.Message);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        private AnnualTurnoverMismatchEvaluator Evaluator()
        {
            return new AnnualTurnoverMismatchEvaluator();
        }
    }
}
