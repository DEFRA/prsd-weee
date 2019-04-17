namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    public interface IXMLChargeBandCalculatorStrategy
    {
        IXMLChargeBandCalculator GetCalculatorOption(Scheme scheme, int complianceYear);
    }
}