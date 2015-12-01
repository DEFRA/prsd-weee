namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using System.Threading.Tasks;

    public interface IFindMatchingUsersDataAccess
    {
        Task<UserSearchData[]> GetCompetentAuthorityUsers();
        Task<UserSearchData[]> GetOrganisationUsers();
    }
}
