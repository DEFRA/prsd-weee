namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.Admin;
    using Domain.Security;

    public interface IUpdateCompetentAuthorityUserRoleAndStatusDataAccess
    {
        Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId);

        Task<Role> GetRoleOrDefaultAsync(string roleName);

        Task<int> UpdateUserRoleAndStatus(CompetentAuthorityUser user, Role role, UserStatus status);
    }
}
