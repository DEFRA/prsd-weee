namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.AatfReturn;

    public interface IGetAatfsDataAccess
    {
        Task<Aatf> GetAatfById(Guid id);

        Task<List<Aatf>> GetAatfs();

        Task<List<Aatf>> GetFilteredAatfs(AatfFilter filter);
    }
}