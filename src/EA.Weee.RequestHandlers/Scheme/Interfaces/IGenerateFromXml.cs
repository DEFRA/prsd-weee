namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Domain.Producer;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public interface IGenerateFromXml
    {
        Task<IEnumerable<ProducerSubmission>> GenerateProducers(ProcessXmlFile messageXmlFile, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges);

        MemberUpload GenerateMemberUpload(ProcessXmlFile messageXmlFile, List<MemberUploadError> errors,
            decimal totalCharges, Scheme scheme, bool hasAnnualCharge);

        Task EnsureProducerRegistrationNumberExists(string producerRegistrationNumber);
    }
}
