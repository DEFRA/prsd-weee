namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;
    using MemberRegistration;
    using Requests.Scheme.MemberRegistration;

    public interface IGenerateFromXml
    {
        Task<IEnumerable<Producer>> GenerateProducers(ProcessXMLFile messageXmlFile, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges);

        MemberUpload GenerateMemberUpload(ProcessXMLFile messageXmlFile, List<MemberUploadError> errors,
            decimal totalCharges, Guid schemeId);
    }
}
