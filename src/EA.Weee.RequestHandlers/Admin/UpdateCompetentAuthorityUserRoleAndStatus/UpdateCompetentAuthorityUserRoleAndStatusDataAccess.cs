namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserRoleAndStatus
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Shared;
    using DataAccess;
    using Domain.Admin;
    using Domain.Security;

    public class UpdateCompetentAuthorityUserRoleAndStatusDataAccess : IUpdateCompetentAuthorityUserRoleAndStatusDataAccess
    {
        private readonly WeeeContext context;

        public UpdateCompetentAuthorityUserRoleAndStatusDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId)
        {
            return await context.CompetentAuthorityUsers.SingleOrDefaultAsync(u => u.Id == competentAuthorityUserId);
        }

        public Task<Role> GetRoleOrDefaultAsync(string roleName)
        {
            return context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<int> UpdateUserRoleAndStatus(CompetentAuthorityUser user, Role role, UserStatus status)
        {
            user.UpdateUserStatus(status.ToDomainEnumeration<Domain.User.UserStatus>());
            user.UpdateRole(role);

            return await context.SaveChangesAsync();
        }
    }
}
