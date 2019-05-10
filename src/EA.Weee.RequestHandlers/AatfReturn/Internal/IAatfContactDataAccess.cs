namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;

    public interface IAatfContactDataAccess
    {
        Task<AatfContact> GetContact(Guid aatfId);

        Task Update(AatfContact oldDetails, AatfContactData newDetails, Country country);
    }
}
