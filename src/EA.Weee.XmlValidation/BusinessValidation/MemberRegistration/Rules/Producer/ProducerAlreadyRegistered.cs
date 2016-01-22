namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using BusinessValidation;
    using Domain;
    using Domain.Obligation;
    using QuerySets;
    using Xml.MemberRegistration;

    public class ProducerAlreadyRegistered : IProducerAlreadyRegistered
    {
        private readonly IProducerQuerySet querySet;

        public ProducerAlreadyRegistered(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(schemeType scheme, producerType producer, Guid organisationId)
        {
            if (producer.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetProducerForOtherSchemeAndObligationType(producer.registrationNo,
                        scheme.complianceYear, organisationId, producer.obligationType.ToDomainObligationType());

                if (existingProducer != null)
                {
                    string schemeName = "another scheme";
                    if (existingProducer.RegisteredProducer.Scheme != null
                        && !string.IsNullOrEmpty(existingProducer.RegisteredProducer.Scheme.SchemeName))
                    {
                        schemeName = existingProducer.RegisteredProducer.Scheme.SchemeName;
                    }
           
                    var errorMessage = string.Format(
                        "{0} {1} is already registered with {2} with obligation type: {3}. Review your file.",
                        producer.GetProducerName(),
                        producer.registrationNo,
                        schemeName,
                        (ObligationType)existingProducer.ObligationType);
                    return RuleResult.Fail(errorMessage);
                }
        
                return RuleResult.Pass();
            }

            return RuleResult.Pass();
        }
    }
}
