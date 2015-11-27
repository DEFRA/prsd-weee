namespace EA.Weee.RequestHandlers.Organisations
{
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System.Linq;
    using System.Threading.Tasks;

    internal class VerifyOrganisationExistsAndIncompleteHandler : IRequestHandler<VerifyOrganisationExistsAndIncomplete, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;

        public VerifyOrganisationExistsAndIncompleteHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public Task<bool> HandleAsync(VerifyOrganisationExistsAndIncomplete message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisationExistsAndIncomplete = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId && o.OrganisationStatus.Value == OrganisationStatus.Incomplete.Value) != null;

            return Task.FromResult(organisationExistsAndIncomplete);
        }
    }
}