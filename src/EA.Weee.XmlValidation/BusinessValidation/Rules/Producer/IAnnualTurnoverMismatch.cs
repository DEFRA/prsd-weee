namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.Schemas;

    public interface IAnnualTurnoverMismatch
    {
        RuleResult Evaluate(producerType producer);
    }
}
