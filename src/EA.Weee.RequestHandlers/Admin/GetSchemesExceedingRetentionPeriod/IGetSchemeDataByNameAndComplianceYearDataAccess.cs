namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSchemeDataByNameAndComplianceYearDataAccess
    {
        Task<List<SchemeDataExceedingRetentionPeriod>> GetItemsAsync(GetSchemeDataExceedingRetentionPeriodRequest request);
    }
}
