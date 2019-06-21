namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IFetchObligatedWeeeForReturnDataAccess
    {
        Task<List<WeeeReceivedAmount>> FetchObligatedWeeeReceivedForReturn(Guid returnId);

        Task<List<WeeeReusedAmount>> FetchObligatedWeeeReusedForReturn(Guid returnId);

        Task<List<WeeeSentOnAmount>> FetchObligatedWeeeSentOnForReturn(Guid weeeSentOnId);

        Task<List<WeeeSentOnAmount>> FetchObligatedWeeeSentOnForReturnByReturn(Guid returnId);
    }
}
