namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface INonObligatedDataAccess
    {
        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);

        Task UpdateAmount(NonObligatedWeee amount, decimal? tonnage);
    }
}
