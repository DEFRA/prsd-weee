namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    internal interface IAddNonObligatedDataAccess
    {
        Task Submit(IEnumerable<NonObligatedWeee> nonObligated);
    }
}
