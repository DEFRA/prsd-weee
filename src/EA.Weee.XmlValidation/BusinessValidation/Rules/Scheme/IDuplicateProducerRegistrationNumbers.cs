namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System.Collections.Generic;
    using BusinessValidation;
    using Xml.Schemas;

    public interface IDuplicateProducerRegistrationNumbers
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
