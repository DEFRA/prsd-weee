namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Xml.MemberRegistration;

    public interface ICompanyAlreadyRegistered
    {
        RuleResult Evaluate(producerType element);
    }
}
