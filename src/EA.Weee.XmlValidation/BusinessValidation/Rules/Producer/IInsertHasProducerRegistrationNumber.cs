namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.Schemas;

    public interface IInsertHasProducerRegistrationNumber
    {
        RuleResult Evaluate(producerType producer);
    }
}
