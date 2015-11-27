namespace EA.Weee.RequestHandlers.Admin
{
    using DataAccess;
    using Domain.Admin;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class GetAdminUserDataAccess : IGetAdminUserDataAccess
    {
        private readonly WeeeContext context;

        public GetAdminUserDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<CompetentAuthorityUser> GetAdminUserOrDefault(Guid userId)
        {
            var competentAuthorityUser = await context.CompetentAuthorityUsers.FirstOrDefaultAsync(u => u.UserId == userId.ToString());
            return competentAuthorityUser;
        }
    }
}
