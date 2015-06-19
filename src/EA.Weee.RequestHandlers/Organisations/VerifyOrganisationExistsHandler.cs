namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;

    internal class VerifyOrganisationExistsHandler : IRequestHandler<VerifyOrganisationExists, bool>
    {
        private readonly WeeeContext context;

        public VerifyOrganisationExistsHandler(WeeeContext context)
        {
            this.context = context;
        }

        public Task<bool> HandleAsync(VerifyOrganisationExists message)
        {
            var organisationExists = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId) != null;

            return Task.FromResult(organisationExists);
        }
    }
}
