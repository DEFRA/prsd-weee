namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Weee.Core.Admin;
    using System;
    using System.Threading.Tasks;

    public interface IFindMatchingUsersDataAccess
    {
        Task<UserSearchData[]> GetCompetentAuthorityUsers();
        Task<UserSearchData[]> GetOrganisationUsers();
    }
}
