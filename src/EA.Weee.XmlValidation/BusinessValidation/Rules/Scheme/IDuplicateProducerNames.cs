namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System.Collections.Generic;
    using BusinessValidation;
    using schemeType = Xml.MemberRegistration.schemeType;

    public interface IDuplicateProducerNames
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
