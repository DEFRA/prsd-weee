namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IFetchNonObligatedWeeeForReturnDataAccess
    {
        Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf);
        Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId);
    }
}
