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

        /// <summary>
        /// Gets the Direct Registrant charge for the specified compliance year and location,
        /// using only the latest record (backwards compatible method).
        /// </summary>
        Task<DirectRegistrantCharge> GetDirectRegistrantChargeByComplianceYear(int complianceYear, bool isNonuk);

        /// <summary>
        /// Gets the applicable Direct Registrant charge based on compliance year, 
        /// location (IsNonUk flag), and the effective date.
        /// Returns the charge with the latest EffectiveFrom date that is on or before the asOfDate.
        /// </summary>
        Task<DirectRegistrantCharge> GetDirectRegistrantChargeAsync(
            int complianceYear,
            bool isNonUk,
            DateTime asOfDate);
    }
}