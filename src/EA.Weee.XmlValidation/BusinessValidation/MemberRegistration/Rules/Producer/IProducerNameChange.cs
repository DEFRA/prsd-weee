namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using System;
    using Xml.MemberRegistration;

    public interface IProducerNameChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid schemeId);
    }
}
