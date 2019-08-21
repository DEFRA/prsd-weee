namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using Core.Shared;
    using Domain.Admin;
    using Domain.Security;
    using System;
    using System.Threading.Tasks;

    public interface IUpdateCompetentAuthorityUserRoleAndStatusDataAccess
    {
        Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId);

        Task<Role> GetRoleOrDefaultAsync(string roleName);

        Task<int> UpdateUserRoleAndStatus(CompetentAuthorityUser user, Role role, UserStatus status);
    }
}
