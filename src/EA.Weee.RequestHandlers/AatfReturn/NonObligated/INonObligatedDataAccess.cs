namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INonObligatedDataAccess
    {
        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);

        Task UpdateAmount(NonObligatedWeee amount, decimal? tonnage);

        Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf);

        Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId);
    }
}
