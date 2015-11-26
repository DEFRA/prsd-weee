namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using BusinessValidation;
    using Xml.MemberUpload;
    using schemeType = Xml.MemberUpload.schemeType;

    public interface IProducerAlreadyRegistered
    {
        RuleResult Evaluate(schemeType scheme, producerType producer, Guid schemeId);
    }
}
