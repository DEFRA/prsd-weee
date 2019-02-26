namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;
    using MemberRegistration;
    using Requests.Scheme.MemberRegistration;

    public interface IGenerateFromXml
    {
        Task<IEnumerable<ProducerSubmission>> GenerateProducers(ProcessXmlFile messageXmlFile, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges);

        MemberUpload GenerateMemberUpload(ProcessXmlFile messageXmlFile, List<MemberUploadError> errors,
            decimal totalCharges, Scheme scheme, bool hasAnnualCharge);
    }
}
