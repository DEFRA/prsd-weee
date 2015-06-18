namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;

    internal class JoinOrganisationHandler : IRequestHandler<JoinOrganisation, Guid>
    {
        private readonly WeeeContext context;

        public JoinOrganisationHandler(WeeeContext context)
        {
            this.context = context;
        }

        public Task<Guid> HandleAsync(JoinOrganisation message)
        {
            var organisation = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}", message.OrganisationId));
            }

            return Task.FromResult(Guid.NewGuid()); // not worrying about actual implementation of this juuuust yet
        }
    }
}
