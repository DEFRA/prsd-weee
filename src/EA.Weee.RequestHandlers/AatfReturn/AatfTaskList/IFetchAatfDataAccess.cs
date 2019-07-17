namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IFetchAatfDataAccess
    {
        Task<List<Aatf>> FetchAatfByReturnQuarterWindow(Return @return);

        Task<List<Aatf>> FetchAatfByReturnId(Guid returnId);
    }
}
