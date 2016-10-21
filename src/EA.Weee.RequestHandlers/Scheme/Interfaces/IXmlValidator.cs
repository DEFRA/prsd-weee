namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;

    public interface IXMLValidator
    {
        Task<IEnumerable<MemberUploadError>> Validate(ProcessXmlFile message);
    }
}
