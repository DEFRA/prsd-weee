namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IGetAatfRetentionPeriodComplianceYearsDataAccess
    {
        Task<List<int>> GetAatfAeRetentionPeriodComplianceYears(int retentionPeriod);
    }
}
