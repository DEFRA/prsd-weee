namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IAatfDataAccess
    {
        Task<Aatf> GetDetails(Guid id);

        Task UpdateDetails(Aatf oldDetails, Aatf newDetails);

        Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country);

        Task<AatfContact> GetContact(Guid aatfId);

        Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country);

        Task<bool> HasAatfData(Guid aatfId);

        Task<bool> HasAatfOrganisationOtherAeOrAatf(Aatf aatf);

        Task RemoveAatf(Guid aatfId);

        Task RemoveAatfData(Aatf aatf, IEnumerable<int> quarters);
    }
}
