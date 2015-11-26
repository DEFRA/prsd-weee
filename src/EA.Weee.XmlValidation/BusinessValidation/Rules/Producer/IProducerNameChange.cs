namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using BusinessValidation;
    using Xml.MemberUpload;
    using schemeType = Xml.MemberUpload.schemeType;

    public interface IProducerNameChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid schemeId);
    }
}
