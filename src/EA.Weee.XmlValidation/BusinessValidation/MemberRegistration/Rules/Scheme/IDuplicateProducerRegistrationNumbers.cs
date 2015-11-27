namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using BusinessValidation;
    using System.Collections.Generic;
    using Xml.MemberRegistration;

    public interface IDuplicateProducerRegistrationNumbers
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
