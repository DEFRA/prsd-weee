namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;

    public interface IGetAllComplianceYearsDataAccess
    {
        Task<List<int>> GetAllComplianceYears(ComplianceYearFor complianceYearFor);
    }
}
