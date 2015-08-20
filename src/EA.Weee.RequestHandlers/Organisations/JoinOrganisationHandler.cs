namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class JoinOrganisationHandler : IRequestHandler<JoinOrganisation, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public JoinOrganisationHandler(IWeeeAuthorization authorization, WeeeContext context, IUserContext userContext)
        {
            this.authorization = authorization;
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(JoinOrganisation message)
        {
            authorization.EnsureCanAccessExternalArea();

            var userId = userContext.UserId;

            if (await context.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()) == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", userId));
            }

            if (await context.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(userId, message.OrganisationId,
                Domain.UserStatus.Pending);

            context.OrganisationUsers.Add(organisationUser);

            await context.SaveChangesAsync();

            return message.OrganisationId;
        }
    }
}
