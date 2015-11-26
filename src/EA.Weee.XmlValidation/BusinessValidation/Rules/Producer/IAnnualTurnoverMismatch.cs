namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IAnnualTurnoverMismatch
    {
        RuleResult Evaluate(producerType producer);
    }
}
