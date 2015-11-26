namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberUpload;

    public interface IUkBasedAuthorisedRepresentative
    {
        RuleResult Evaluate(producerType producer);
    }
}
