namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using System;
    using System.Threading.Tasks;

    public interface IGetManageUserDataAccess
    {
        Task<ManageUserData> GetCompetentAuthorityUser(Guid id);
        Task<ManageUserData> GetOrganisationUser(Guid id);
    }
}
