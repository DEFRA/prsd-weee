namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using Core.Shared;
    using Core.XmlBusinessValidation;
    using Extensions;
    using QuerySets;
    using Rules;

    public class ProducerNameWarningEvaluator : IRule<ProducerNameWarning>
    {
        private readonly IProducerNameWarningQuerySet querySet;

        public ProducerNameWarningEvaluator(IProducerNameWarningQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(ProducerNameWarning rule)
        {
            if (rule.Producer.status == statusType.A)
            {
                var existingProducerName = string.Empty;

                var existingProducer =
                    querySet.GetLatestProducerForComplianceYearAndScheme(rule.Producer.registrationNo,
                        rule.Scheme.complianceYear, rule.OrganisationId);

                if (existingProducer == null)
                {
                    existingProducer =
                        querySet.GetLatestProducerFromPreviousComplianceYears(rule.Producer.registrationNo);
                }

                if (existingProducer != null)
                {
                    existingProducerName = existingProducer.OrganisationName;
                }
                else
                {
                    var existingMigratedProducer = querySet.GetMigratedProducer(rule.Producer.registrationNo);

                    if (existingMigratedProducer == null)
                    {
                        // Producer doesn't exist so no warning
                        return RuleResult.Pass();
                    }

                    existingProducerName = existingMigratedProducer.ProducerName;
                }

                if (existingProducerName != rule.Producer.GetProducerName())
                {
                    var errorMessage = string.Format("The company name of producer (registration number {0})  will change from {1} to {2}. Please double-check that this is correct.", 
                        rule.Producer.registrationNo, existingProducerName, rule.Producer.GetProducerName());

                    return RuleResult.Fail(errorMessage, ErrorLevel.Warning);
                }
            }

            return RuleResult.Pass();
        }
    }
}
