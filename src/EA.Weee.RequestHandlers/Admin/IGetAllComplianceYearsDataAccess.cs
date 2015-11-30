namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAllComplianceYearsDataAccess
    {
        Task<List<int>> GetAllComplianceYears();
    }
}
