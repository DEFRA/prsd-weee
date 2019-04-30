namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;

    public interface IAatfSiteDataAccess
    {
        Task Submit(WeeeReusedSite weeeReusedSite);

        Task<List<AatfAddress>> GetAddresses(Guid aatfId, Guid returnId);

        Task<List<WeeeReusedAmount>> GetObligatedWeeeForReturnAndAatf(Guid aatfId, Guid returnId);

        Task Update(AatfAddress oldAddress, AddressData newAddress, Country country);
    }
}
