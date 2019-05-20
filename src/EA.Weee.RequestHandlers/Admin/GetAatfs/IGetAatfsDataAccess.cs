namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IGetAatfsDataAccess
    {
        Task<Domain.AatfReturn.Aatf> GetAatfById(Guid id);

        Task<List<Domain.AatfReturn.Aatf>> GetAatfs();
    }
}