namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using Xml.MemberRegistration;

    public interface IMemberRegistrationBusinessValidator
    {
        IEnumerable<RuleResult> Validate(schemeType scheme, Guid schemeId);
    }
}
