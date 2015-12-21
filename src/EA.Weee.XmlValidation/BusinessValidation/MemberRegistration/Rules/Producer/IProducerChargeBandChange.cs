namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using Xml.MemberRegistration;

    public interface IProducerChargeBandChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid organisationId);
    }
}