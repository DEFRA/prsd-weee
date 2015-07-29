namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.GenerateProducerObjects
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.PCS;
    using Domain.Producer;
    using Requests.PCS.MemberRegistration;

    public interface IGenerateFromXml
    {
        Task<IEnumerable<Producer>> Generate(ProcessXMLFile messageXmlFile, MemberUpload memberUpload);
    }
}
