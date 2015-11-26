namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberUpload;

    public interface IAmendmentHasNoProducerRegistrationNumber
    {
        RuleResult Evaluate(producerType producer);
    }
}
