namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;
    
    internal class VerifyOrganisationExistsAndIncompleteHandler : IRequestHandler<VerifyOrganisationExistsAndIncomplete, bool>
    {
        private readonly WeeeContext context;

        public VerifyOrganisationExistsAndIncompleteHandler(WeeeContext context)
        {
            this.context = context;
        }

        public Task<bool> HandleAsync(VerifyOrganisationExistsAndIncomplete message)
        {
            var organisationExistsAndIncomplete = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId && o.OrganisationStatus.Value == OrganisationStatus.Incomplete.Value) != null;

            return Task.FromResult(organisationExistsAndIncomplete);
        }
    }
}