namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INonObligatedDataAccess
    {
        Task Remove(IEnumerable<NonObligatedWeee> nonObligated);

        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);

        Task UpdateAmount(NonObligatedWeee amount, decimal? tonnage);

        Task UpdateAmountForIds(IEnumerable<Tuple<Guid, decimal?>> amounts);

        Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf);

        Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId);
    }
}
