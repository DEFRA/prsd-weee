namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAatfContactDataAccess
    {
        Task<AatfContact> GetContact(Guid aatfId);
    }
}
