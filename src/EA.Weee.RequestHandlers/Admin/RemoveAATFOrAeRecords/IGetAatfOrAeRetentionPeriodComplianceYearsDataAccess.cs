namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IGetAatfOrAeRetentionPeriodComplianceYearsDataAccess
    {
        Task<List<int>> GetAatfAeRetentionPeriodComplianceYears(FacilityType faclityType, int retentionPeriod);
    }
}
