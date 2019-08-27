namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IXMLValidator
    {
        Task<IEnumerable<MemberUploadError>> Validate(ProcessXmlFile message);
    }
}
