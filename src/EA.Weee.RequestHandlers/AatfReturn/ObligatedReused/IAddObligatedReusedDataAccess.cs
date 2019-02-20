namespace EA.Weee.RequestHandlers.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddObligatedReusedDataAccess
    {
        Task Submit(IEnumerable<WeeeReusedAmount> aatfWeeeReceivedAmount);
        Task<Guid> GetSchemeId(Guid organisationId);
        Task<Guid> GetAatfId(Guid organisationId);
    }
}