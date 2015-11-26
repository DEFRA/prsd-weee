namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberUpload;

    public interface IAnnualTurnoverMismatch
    {
        RuleResult Evaluate(producerType producer);
    }
}
