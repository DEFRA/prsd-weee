namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.Schemas;

    public interface IUkBasedAuthorisedRepresentative
    {
        RuleResult Evaluate(producerType producer);
    }
}
