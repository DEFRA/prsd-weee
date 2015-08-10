namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;

    public interface IGenerateFromXml
    {
        Task<IEnumerable<Producer>> Generate(ProcessXMLFile messageXmlFile, MemberUpload memberUpload, Hashtable producerCharges);
    }
}
