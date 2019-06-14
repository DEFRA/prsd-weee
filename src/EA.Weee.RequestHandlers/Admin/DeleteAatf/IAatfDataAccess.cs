namespace EA.Weee.RequestHandlers.Admin.DeleteAatf
{
    using System;
    using System.Threading.Tasks;

    public interface IAatfDataAccess
    {
        Task<bool> DoesAatfHaveData(Guid aatfId);

        Task<bool> DoesAatfOrganisationHaveMoreAatfs(Guid aatfId);

        Task<bool> DoesAatfOrganisationHaveActiveUsers(Guid aatfId);
    }
}
