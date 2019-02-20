namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IFetchNonObligatedWeeeForReturnDataAccess
    {
        Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf);
        Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId);
    }
}
