namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System.Collections.Generic;
    using BusinessValidation;
    using schemeType = Xml.MemberUpload.schemeType;

    public interface IDuplicateProducerRegistrationNumbers
    {
        IEnumerable<RuleResult> Evaluate(schemeType scheme);
    }
}
