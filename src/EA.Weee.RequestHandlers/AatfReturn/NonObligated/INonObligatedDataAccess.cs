namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    internal interface INonObligatedDataAccess
    {
        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);
    }
}
