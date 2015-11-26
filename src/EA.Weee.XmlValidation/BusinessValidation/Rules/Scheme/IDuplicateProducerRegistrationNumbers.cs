namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System.Collections.Generic;
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IDuplicateProducerRegistrationNumbers
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
