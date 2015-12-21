namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using BusinessValidation;
    using Xml.MemberRegistration;

    public interface IUkBasedAuthorisedRepresentative
    {
        RuleResult Evaluate(producerType producer);
    }
}
