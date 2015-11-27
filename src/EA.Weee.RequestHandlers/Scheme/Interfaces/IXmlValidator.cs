namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;

    public interface IXmlValidator
    {
        IEnumerable<MemberUploadError> Validate(ProcessXMLFile message);
    }
}
