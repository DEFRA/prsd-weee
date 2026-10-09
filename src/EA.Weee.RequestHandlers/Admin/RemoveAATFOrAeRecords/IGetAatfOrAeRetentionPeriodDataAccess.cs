namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using Domain.AatfReturn;
    using EA.Weee.Core.Admin;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAatfOrAeRetentionPeriodDataAccess
    {
        Task<List<Aatf>> GetFilteredAatfs(RemoveAatfOrAeFilter filter);
    }
}
