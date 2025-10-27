namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using Xml.MemberRegistration;

    public interface IProducerSellingTechniqueChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid organisationId);
    }
}
