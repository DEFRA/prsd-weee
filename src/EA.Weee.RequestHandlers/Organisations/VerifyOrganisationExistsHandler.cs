namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;
    using Security;

    internal class VerifyOrganisationExistsHandler : IRequestHandler<VerifyOrganisationExists, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;

        public VerifyOrganisationExistsHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public Task<bool> HandleAsync(VerifyOrganisationExists message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationId);

            var organisationExists = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId) != null;

            return Task.FromResult(organisationExists);
        }
    }
}
