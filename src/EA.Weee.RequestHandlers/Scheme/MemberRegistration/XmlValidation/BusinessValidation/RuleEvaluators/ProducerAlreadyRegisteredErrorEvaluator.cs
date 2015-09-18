namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using Core.XmlBusinessValidation;
    using Extensions;
    using QuerySets;
    using Rules;

    public class ProducerAlreadyRegisteredErrorEvaluator : IRule<ProducerAlreadyRegisteredError>
    {
        private readonly IProducerQuerySet querySet;

        public ProducerAlreadyRegisteredErrorEvaluator(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(ProducerAlreadyRegisteredError rule)
        {
            if (rule.Producer.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetProducerForOtherSchemeAndObligationType(rule.Producer.registrationNo,
                        rule.Scheme.complianceYear, rule.OrganisationId, (int)rule.Producer.obligationType.ToDomainObligationType());

                if (existingProducer != null)
                {
                    string schemeName = "another scheme";
                    if (existingProducer.Scheme != null && !string.IsNullOrEmpty(existingProducer.Scheme.SchemeName))
                    {
                        schemeName = existingProducer.Scheme.SchemeName;
                    }
           
                    var errorMessage = string.Format(
                        "{0} {1} is already registered with {2} with obligation type: {3}.",
                        rule.Producer.GetProducerName(),
                        rule.Producer.registrationNo,
                        schemeName,
                        rule.Producer.obligationType);
                    return RuleResult.Fail(errorMessage);
                }
        
                return RuleResult.Pass();
            }

            return RuleResult.Pass();
        }
    }
}
