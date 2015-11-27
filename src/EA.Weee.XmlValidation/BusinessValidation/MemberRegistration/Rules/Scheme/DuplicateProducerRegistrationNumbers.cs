namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessValidation;
    using Xml.MemberRegistration;

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
                                    "Producer registration number (PRN) {0} can only appear once in your XML file. Review your file.",
                                    producer.registrationNo));
                        continue;
                    }
                }

                yield return RuleResult.Pass();
            }
        }
    }
}
