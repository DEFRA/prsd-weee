namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using BusinessValidation;
    using QuerySets;
    using Xml;
    using Xml.Schemas;

    public class ProducerAlreadyRegistered : IProducerAlreadyRegistered
    {
        private readonly IProducerQuerySet querySet;

        public ProducerAlreadyRegistered(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(schemeType scheme, producerType producer, Guid schemeId)
        {
            if (producer.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetProducerForOtherSchemeAndObligationType(producer.registrationNo,
                        scheme.complianceYear, schemeId, (int)producer.obligationType.ToDomainObligationType());

                if (existingProducer != null)
                {
                    string schemeName = "another scheme";
                    if (existingProducer.Scheme != null && !string.IsNullOrEmpty(existingProducer.Scheme.SchemeName))
                    {
                        schemeName = existingProducer.Scheme.SchemeName;
                    }
           
                    var errorMessage = string.Format(
                        "{0} {1} is already registered with {2} with obligation type: {3}.",
                        producer.GetProducerName(),
                        producer.registrationNo,
                        schemeName,
                        producer.obligationType);
                    return RuleResult.Fail(errorMessage);
                }
        
                return RuleResult.Pass();
            }

            return RuleResult.Pass();
        }
    }
}
