namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using System.Collections.Generic;
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IDuplicateProducerNames
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
