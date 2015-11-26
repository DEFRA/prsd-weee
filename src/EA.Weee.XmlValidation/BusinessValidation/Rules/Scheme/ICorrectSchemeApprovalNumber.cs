namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System;
    using BusinessValidation;
    using schemeType = Xml.MemberUpload.schemeType;

    public interface ICorrectSchemeApprovalNumber
    {
        RuleResult Evaluate(schemeType scheme, Guid schemeId);
    }
}