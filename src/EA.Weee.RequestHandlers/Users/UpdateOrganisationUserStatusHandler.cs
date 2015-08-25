namespace EA.Weee.RequestHandlers.Users
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Users;
    using Security;

    internal class UpdateOrganisationUserStatusHandler : IRequestHandler<UpdateOrganisationUserStatus, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public UpdateOrganisationUserStatusHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateOrganisationUserStatus query)
        {
            authorization.EnsureOrganisationAccess(query.OrganisationId);

            var organisationUser =
                await context.OrganisationUsers.SingleOrDefaultAsync(ou => ou.OrganisationId == query.OrganisationId && ou.UserId == query.UserId);

            var userStatus = ValueObjectInitializer.GetUserStatus(query.UserStatus);
            organisationUser.UpdateUserStatus(userStatus);
            await context.SaveChangesAsync();
            return organisationUser.OrganisationId;
        }
    }
}
