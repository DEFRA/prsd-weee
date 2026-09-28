namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using Domain.AatfReturn;
    using EA.Weee.Core.Admin;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAatfsRetentionPeriodDataAccess
    {
        Task<List<Aatf>> GetFilteredAatfs(RemoveAATFFilter filter);
    }
}
