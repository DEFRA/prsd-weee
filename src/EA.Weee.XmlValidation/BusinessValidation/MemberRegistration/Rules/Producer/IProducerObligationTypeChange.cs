namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Xml.MemberRegistration;

    public interface IProducerObligationTypeChange
    {
        RuleResult Evaluate(producerType producer);
    }
}
