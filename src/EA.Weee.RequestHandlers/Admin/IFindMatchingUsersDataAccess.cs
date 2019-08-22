namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using EA.Weee.Core.User;
    using System.Threading.Tasks;

    public interface IFindMatchingUsersDataAccess
    {
        Task<UserSearchData[]> GetCompetentAuthorityUsers(UserFilter filter);

        Task<UserSearchData[]> GetOrganisationUsers(UserFilter filter);
    }
}
