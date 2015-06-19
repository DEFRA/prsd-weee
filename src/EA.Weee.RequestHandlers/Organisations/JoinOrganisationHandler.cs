namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.Organisations;

    internal class JoinOrganisationHandler : IRequestHandler<JoinOrganisation, Guid>
    {
        private readonly WeeeContext context;

        public JoinOrganisationHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(JoinOrganisation message)
        {
            if (context.Users.FirstOrDefault(u => u.Id == message.UserId) == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", message.UserId));
            }

            if (context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}", message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(message.UserId, message.OrganisationId, OrganisationUserStatus.Pending);

            context.OrganisationUsers.Add(organisationUser);

            await context.SaveChangesAsync();

            return message.OrganisationId;
        }
    }
}
