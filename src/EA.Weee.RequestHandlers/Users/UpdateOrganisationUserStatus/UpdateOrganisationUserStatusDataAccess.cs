namespace EA.Weee.RequestHandlers.Users.UpdateOrganisationUserStatus
{
    using Core.Helpers;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class UpdateOrganisationUserStatusDataAccess : IUpdateOrganisationUserStatusDataAccess
    {
        private readonly WeeeContext context;

        public UpdateOrganisationUserStatusDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<OrganisationUser> GetOrganisationUser(Guid organisationUserId)
        {
            return await context.OrganisationUsers.SingleOrDefaultAsync(ou => ou.Id == organisationUserId);
        }

        public async Task<int> ChangeOrganisationUserStatus(OrganisationUser organisationUser, Core.Shared.UserStatus status)
        {
            organisationUser.UpdateUserStatus(status.ToDomainEnumeration<UserStatus>());
            return await context.SaveChangesAsync();
        }
    }
}
