namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Threading.Tasks;
    public interface IAnnualChargeDataAccess
    {
        /// <summary>
        /// Gets the annual charge amount for a specific competent authority and compliance year.
        /// </summary>
        /// <param name="competentAuthorityId">The competent authority ID</param>
        /// <param name="complianceYear">The compliance year</param>
        /// <returns>The annual charge amount, or null if not found</returns>
        Task<decimal?> GetAnnualChargeForComplianceYear(Guid competentAuthorityId, int complianceYear);
    }
}
