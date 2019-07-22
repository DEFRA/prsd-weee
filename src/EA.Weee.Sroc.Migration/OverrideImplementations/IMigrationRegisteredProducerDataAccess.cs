namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;
    using Xml.MemberRegistration;

    public interface IMigrationRegisteredProducerDataAccess
    {
        void Add(RegisteredProducer registeredProducer);

        Task<RegisteredProducer> GetProducerRegistration(Guid id);

        Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear);

        ProducerSubmission GetProducerRegistration(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber,
            MemberUpload memberUpload);

        bool HasPreviousAmendmentCharge(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, MemberUpload memberUpload);

        ProducerSubmission GetProducerRegistrationForInsert(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber,
            MemberUpload upload, string name, producerType producerType);
    }
}
