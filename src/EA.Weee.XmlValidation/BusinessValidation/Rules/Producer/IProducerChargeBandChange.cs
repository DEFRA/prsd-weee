namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using EA.Weee.Xml.MemberRegistration;
    using System;

    public interface IProducerChargeBandChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid schemeId);
    }
}