namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    internal interface IAddNonObligatedDataAccess
    {
        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);
    }
}
