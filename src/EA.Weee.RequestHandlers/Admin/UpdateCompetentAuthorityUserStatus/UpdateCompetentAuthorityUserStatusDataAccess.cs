namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Shared;
    using DataAccess;
    using Domain.Admin;

    public class UpdateCompetentAuthorityUserStatusDataAccess : IUpdateCompetentAuthorityUserStatusDataAccess
    {
        private readonly WeeeContext context;

        public UpdateCompetentAuthorityUserStatusDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId)
        {
            return await context.CompetentAuthorityUsers.SingleOrDefaultAsync(u => u.Id == competentAuthorityUserId);
        }

        public async Task<int> UpdateCompetentAuthorityUserStatus(CompetentAuthorityUser user, UserStatus status)
        {
            user.UpdateUserStatus(status.ToDomainEnumeration<Domain.User.UserStatus>());
            return await context.SaveChangesAsync();
        }
    }
}
