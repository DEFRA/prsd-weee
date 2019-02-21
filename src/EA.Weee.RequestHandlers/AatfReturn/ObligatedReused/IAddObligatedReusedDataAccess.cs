namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddObligatedReusedDataAccess
    {
        Task Submit(IEnumerable<WeeeReusedAmount> aatfWeeeReusedAmount);
        Task<Guid> GetAatfId(Guid organisationId);
    }
}