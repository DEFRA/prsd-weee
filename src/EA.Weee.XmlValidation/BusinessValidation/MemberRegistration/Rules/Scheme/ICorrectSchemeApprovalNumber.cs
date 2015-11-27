﻿namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using System;
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface ICorrectSchemeApprovalNumber
    {
        RuleResult Evaluate(schemeType scheme, Guid schemeId);
    }
}