namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Xml.MemberRegistration;

    public interface ICompanyRegistrationNumberChange
    {
        RuleResult Evaluate(producerType element);
    }
}
