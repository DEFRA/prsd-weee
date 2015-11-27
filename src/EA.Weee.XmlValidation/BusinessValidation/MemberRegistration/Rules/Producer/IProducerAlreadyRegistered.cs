namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IProducerAlreadyRegistered
    {
        RuleResult Evaluate(schemeType scheme, producerType producer, Guid schemeId);
    }
}
