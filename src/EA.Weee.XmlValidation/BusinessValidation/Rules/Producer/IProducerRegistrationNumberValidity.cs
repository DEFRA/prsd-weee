namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.Schemas;

    public interface IProducerRegistrationNumberValidity
    {
        RuleResult Evaluate(producerType producer);
    }
}
