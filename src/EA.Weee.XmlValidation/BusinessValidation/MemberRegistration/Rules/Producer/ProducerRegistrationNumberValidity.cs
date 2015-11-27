namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using QuerySets;
    using System.Linq;
    using Xml.MemberRegistration;

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
                                "We cannot recognise the producer registration number (PRN) you have entered for {0} {1}. Enter the correct PRN for this producer.",
                                producer.GetProducerName(), producer.registrationNo));
                }
            }

            return RuleResult.Pass();
        }
    }
}
