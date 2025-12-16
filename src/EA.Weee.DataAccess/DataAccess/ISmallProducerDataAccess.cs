namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;

    public interface ISmallProducerDataAccess
    {
        Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionByComplianceYear(Guid directRegistrantId, int complianceYear);

        Task<DirectRegistrant> GetDirectRegistrantByOrganisationId(Guid organisationId);

        Task<DirectRegistrant> GetById(Guid directRegistrantId);

        Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionById(Guid directProducerSubmissionId);

        Task<DirectRegistrantCharge> GetDirectRegistrantChargeByComplianceYear(int complianceYear, bool isEngland);
    }
}