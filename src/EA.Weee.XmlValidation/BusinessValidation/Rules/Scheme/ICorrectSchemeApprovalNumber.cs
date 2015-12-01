namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System;
    using BusinessValidation;
    using Xml.Schemas;

    public interface ICorrectSchemeApprovalNumber
    {
        RuleResult Evaluate(schemeType scheme, Guid schemeId);
    }
}