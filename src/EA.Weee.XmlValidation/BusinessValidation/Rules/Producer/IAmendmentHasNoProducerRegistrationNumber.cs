namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.Schemas;

    public interface IAmendmentHasNoProducerRegistrationNumber
    {
        RuleResult Evaluate(producerType producer);
    }
}
