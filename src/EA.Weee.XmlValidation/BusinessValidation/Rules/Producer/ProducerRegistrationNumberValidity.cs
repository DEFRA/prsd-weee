namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System.Linq;
    using BusinessValidation;
    using QuerySets;
    using Xml;
    using Xml.Schemas;

    public class ProducerRegistrationNumberValidity : IProducerRegistrationNumberValidity
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public ProducerRegistrationNumberValidity(IProducerQuerySet producerQuerySet, IMigratedProducerQuerySet migratedProducerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
            this.migratedProducerQuerySet = migratedProducerQuerySet;
        }

        public RuleResult Evaluate(producerType producer)
        {
            if (producer.status == statusType.A && !string.IsNullOrEmpty(producer.registrationNo))
            {
                var validRegistrationNumbers = producerQuerySet.GetAllRegistrationNumbers()
                    .Union(migratedProducerQuerySet.GetAllRegistrationNumbers())
                    .Select(prn => prn.ToLowerInvariant());

                if (!validRegistrationNumbers.Contains(producer.registrationNo.ToLowerInvariant()))
                {
                    return
                        RuleResult.Fail(
                            string.Format(
                                "{0} {1} has a producer registration number in the xml which is not recognised. In order to register or amend this producer please enter the correct producer registration number for the producer.",
                                producer.GetProducerName(), producer.registrationNo));
                }
            }

            return RuleResult.Pass();
        }
    }
}
