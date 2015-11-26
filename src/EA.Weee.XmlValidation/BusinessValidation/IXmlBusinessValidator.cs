namespace EA.Weee.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using schemeType = Xml.MemberRegistration.schemeType;

    public interface IXmlBusinessValidator
    {
        IEnumerable<RuleResult> Validate(schemeType scheme, Guid schemeId);
    }
}
