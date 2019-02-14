namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFetchObligatedWeeeForReturnDataAccess
    {
        Task<List<WeeeReceivedAmount>> FetchObligatedWeeeForReturn(Guid returnId);
    }
}
