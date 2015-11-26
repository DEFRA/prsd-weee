namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IInsertHasProducerRegistrationNumber
    {
        RuleResult Evaluate(producerType producer);
    }
}
