namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using BusinessValidation;
    using QuerySets;
    using Xml;
    using Xml.Schemas;

    public class ProducerNameChange : IProducerNameChange
    {
        private readonly IProducerQuerySet querySet;

        public ProducerNameChange(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(schemeType scheme, producerType producer, Guid schemeId)
        {
            if (producer.status == statusType.A)
            {
                string existingProducerName = string.Empty;

                var existingProducer =
                    querySet.GetLatestProducerForComplianceYearAndScheme(producer.registrationNo,
                        scheme.complianceYear, schemeId) ??
                    querySet.GetLatestProducerFromPreviousComplianceYears(producer.registrationNo);

                if (existingProducer != null)
                {
                    existingProducerName = existingProducer.OrganisationName;
                }
                else
                {
                    var existingMigratedProducer = querySet.GetMigratedProducer(producer.registrationNo);

                    if (existingMigratedProducer == null)
                    {
                        // Producer doesn't exist so no warning
                        return RuleResult.Pass();
                    }

                    existingProducerName = existingMigratedProducer.ProducerName;
                }

                if (existingProducerName != producer.GetProducerName())
                {
                    var errorMessage = string.Format("The company name of producer (registration number {0})  will change from {1} to {2}. Please double-check that this is correct.",
                        producer.registrationNo, existingProducerName, producer.GetProducerName());

                    return RuleResult.Fail(errorMessage, Core.Shared.ErrorLevel.Warning);
                }
            }

            return RuleResult.Pass();
        }
    }
}
