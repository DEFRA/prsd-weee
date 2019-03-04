namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IGetAatfSiteDataAccess
    {
        Task<List<AatfAddress>> FetchAddresses(Guid aatfId, Guid returnId);
    }
}
