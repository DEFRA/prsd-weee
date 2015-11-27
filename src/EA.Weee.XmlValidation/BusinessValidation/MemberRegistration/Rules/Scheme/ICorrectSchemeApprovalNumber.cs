namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using BusinessValidation;
    using System;
    using Xml.MemberRegistration;

    public interface ICorrectSchemeApprovalNumber
    {
        RuleResult Evaluate(schemeType scheme, Guid schemeId);
    }
}