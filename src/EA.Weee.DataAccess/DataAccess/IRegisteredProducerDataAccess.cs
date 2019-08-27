namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRegisteredProducerDataAccess
    {
        void Add(RegisteredProducer registeredProducer);

        Task<RegisteredProducer> GetProducerRegistration(Guid id);

        Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear);

        Task<RegisteredProducer> GetProducerRegistration(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber);

        bool HasPreviousAmendmentCharge(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber);
    }
}
