namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using EA.Weee.Xml.MemberRegistration;

    public interface ICompanyAlreadyRegistered
    {
        RuleResult Evaluate(producerType element);
    }
}
