namespace EA.Weee.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using Xml.Schemas;

    public interface IXmlBusinessValidator
    {
        IEnumerable<RuleResult> Validate(schemeType scheme, Guid schemeId);
    }
}
