namespace EA.Weee.RequestHandlers.Admin.Submissions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAllComplianceYearsDataAccess
    {
        Task<List<int>> GetAllComplianceYears();
    }
}
