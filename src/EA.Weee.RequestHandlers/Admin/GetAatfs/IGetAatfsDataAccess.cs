namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAatfsDataAccess
    {
        Task<Aatf> GetAatfById(Guid id);

        Task<List<Aatf>> GetAatfs();

        Task<List<Aatf>> GetFilteredAatfs(AatfFilter filter);

        Task<List<Aatf>> GetLatestAatfs();

        Task<List<Aatf>> GetAatfsBySiteAddressId(Guid siteAddressId);
    }
}