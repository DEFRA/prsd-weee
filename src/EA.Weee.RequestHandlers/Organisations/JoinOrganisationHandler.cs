namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.Organisations;

    internal class JoinOrganisationHandler : IRequestHandler<JoinOrganisation, Guid>
    {
        private readonly WeeeContext context;

        private readonly IUserContext userContext;

        public JoinOrganisationHandler(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(JoinOrganisation message)
        {
            var userId = userContext.UserId;

            if (context.Users.FirstOrDefault(u => u.Id == userId.ToString()) == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", userId));
            }

            if (context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}", message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(userId, message.OrganisationId, EA.Weee.Domain.OrganisationUserStatus.Pending);

            context.OrganisationUsers.Add(organisationUser);

            await context.SaveChangesAsync();

            return message.OrganisationId;
        }
    }
}
