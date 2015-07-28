namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using Requests.PCS.MemberRegistration;

    public interface IXmlChargeBandCalculator
    {
        decimal Calculate(ProcessXMLFile message);
    }
}