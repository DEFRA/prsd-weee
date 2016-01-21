namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetMemberRegistrationsActiveComplianceYearsDataAccess
    {
        Task<List<int>> Get();
    }
}
