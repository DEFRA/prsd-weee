namespace EA.Weee.RequestHandlers.Admin
{
    using System.Threading.Tasks;
    using Core.Admin;
    using EA.Weee.Core.User;

    public interface IFindMatchingUsersDataAccess
    {
        Task<UserSearchData[]> GetCompetentAuthorityUsers(UserFilter filter);

        Task<UserSearchData[]> GetOrganisationUsers(UserFilter filter);
    }
}
