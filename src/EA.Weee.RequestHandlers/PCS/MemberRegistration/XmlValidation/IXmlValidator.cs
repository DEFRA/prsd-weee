namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;

    public interface IXmlValidator
    {
        IEnumerable<MemberUploadError> Validate(ProcessXMLFile message);
    }
}
