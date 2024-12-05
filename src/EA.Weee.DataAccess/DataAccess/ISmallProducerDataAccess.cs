namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Threading.Tasks;

    public interface ISmallProducerDataAccess
    {
        Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionByComplianceYear(Guid directRegistrantId, int complianceYear);

        Task<DirectRegistrant> GetDirectRegistrantByOrganisationId(Guid organisationId);

        Task<DirectRegistrant> GetById(Guid directRegistrantId);

        Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionById(Guid directProducerSubmissionId);
    }
}