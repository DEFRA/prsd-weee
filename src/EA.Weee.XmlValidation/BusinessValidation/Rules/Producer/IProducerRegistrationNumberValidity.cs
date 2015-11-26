namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberUpload;

    public interface IProducerRegistrationNumberValidity
    {
        RuleResult Evaluate(producerType producer);
    }
}
