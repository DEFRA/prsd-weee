namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using Requests.PCS.MemberRegistration;

    public interface IXmlConverter
    {
        schemeType Convert(ProcessXmlFile message);
    }
}
