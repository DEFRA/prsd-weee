namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessValidation;
    using Xml.Schemas;

    public class DuplicateProducerRegistrationNumbers : IDuplicateProducerRegistrationNumbers
    {
        public IEnumerable<RuleResult> Evaluate(schemeType scheme)
        {
            var duplicateRegistrationNumbers = new List<string>();

            foreach (var producer in scheme.producerList)
            {
                if (!string.IsNullOrEmpty(producer.registrationNo))
                {
                    var isDuplicate = scheme.producerList
                        .Any(p => p != producer && p.registrationNo == producer.registrationNo);

                    if (isDuplicate && !duplicateRegistrationNumbers.Contains(producer.registrationNo))
                    {
                        duplicateRegistrationNumbers.Add(producer.registrationNo);
                        yield return
                            RuleResult.Fail(
                                string.Format(
                                    "The Registration Number '{0}' appears more than once in the uploaded XML file",
                                    producer.registrationNo));
                    }
                }

                yield return RuleResult.Pass();
            }
        }
    }
}
