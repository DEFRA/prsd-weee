namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddObligatedReceivedDataAccess
    {
        Task Submit(IEnumerable<WeeeReceivedAmount> aatfWeeReceivedAmount);
        Task<Guid> GetSchemeId(Guid organisationId);
        Task<Guid> GetAatfId(Guid organisationId);
    }
}