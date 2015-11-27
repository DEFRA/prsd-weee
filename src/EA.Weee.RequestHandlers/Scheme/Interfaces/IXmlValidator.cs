namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;

    public interface IXmlValidator
    {
        IEnumerable<MemberUploadError> Validate(ProcessXMLFile message);
    }
}
