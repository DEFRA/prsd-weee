﻿namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using BusinessValidation;
    using QuerySets;
    using Xml;
    using Xml.Schemas;

    public class ProducerNameChange : IProducerNameChange
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public ProducerNameChange(IProducerQuerySet producerQuerySet, IMigratedProducerQuerySet migratedProducerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
            this.migratedProducerQuerySet = migratedProducerQuerySet;
        }

        public RuleResult Evaluate(schemeType scheme, producerType producer, Guid schemeId)
        {
            if (producer.status == statusType.A)
            {
                string existingProducerName = string.Empty;

                var existingProducer =
                    producerQuerySet.GetLatestProducerForComplianceYearAndScheme(producer.registrationNo,
                        scheme.complianceYear, schemeId) ??
                    producerQuerySet.GetLatestProducerFromPreviousComplianceYears(producer.registrationNo);

                if (existingProducer != null)
                {
                    existingProducerName = existingProducer.OrganisationName;
                }
                else
                {
                    var existingMigratedProducer = migratedProducerQuerySet.GetMigratedProducer(producer.registrationNo);

                    if (existingMigratedProducer == null)
                    {
                        // Producer doesn't exist so no warning
                        return RuleResult.Pass();
                    }

                    existingProducerName = existingMigratedProducer.ProducerName;
                }

                if (existingProducerName != producer.GetProducerName())
                {
                    var errorMessage = string.Format("The company name of {1} {0} will change from {1} to {2}.",
                        producer.registrationNo, existingProducerName, producer.GetProducerName());

                    return RuleResult.Fail(errorMessage, Core.Shared.ErrorLevel.Warning);
                }
            }

            return RuleResult.Pass();
        }
    }
}
