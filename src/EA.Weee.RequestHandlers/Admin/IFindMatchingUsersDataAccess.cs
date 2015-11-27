namespace EA.Weee.RequestHandlers.Admin
{
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IFindMatchingUsersDataAccess
    {
        Task<UserSearchData[]> GetCompetentAuthorityUsers();
        Task<UserSearchData[]> GetOrganisationUsers();
    }
}
