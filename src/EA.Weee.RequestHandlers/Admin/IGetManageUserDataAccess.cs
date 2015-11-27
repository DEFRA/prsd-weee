namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetManageUserDataAccess
    {
        Task<ManageUserData> GetCompetentAuthorityUser(Guid id);
        Task<ManageUserData> GetOrganisationUser(Guid id);
    }
}
