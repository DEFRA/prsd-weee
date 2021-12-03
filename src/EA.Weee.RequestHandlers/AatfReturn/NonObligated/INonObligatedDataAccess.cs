namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INonObligatedDataAccess
    {
        Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf);

        Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId);

        Task InsertNonObligatedWeee(Guid returnId, IEnumerable<NonObligatedWeee> nonObligated);

        Task UpdateNonObligatedWeeeAmounts(Guid returnId, IEnumerable<Tuple<Guid, decimal?>> amounts);
    }
}
