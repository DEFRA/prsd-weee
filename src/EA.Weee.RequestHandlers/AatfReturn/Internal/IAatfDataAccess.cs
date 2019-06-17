namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;

    public interface IAatfDataAccess
    {
        Task<Aatf> GetDetails(Guid id);

        Task UpdateDetails(Aatf oldDetails, Aatf newDetails);

        Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country);

        Task<AatfContact> GetContact(Guid aatfId);

        Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country);

        Task<bool> DoesAatfHaveData(Guid aatfId);

        Task<bool> DoesAatfOrganisationHaveMoreAatfs(Guid aatfId);

        Task<bool> DoesAatfOrganisationHaveActiveUsers(Guid aatfId);
    }
}
