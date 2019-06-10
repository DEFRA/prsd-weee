namespace EA.Weee.RequestHandlers.Admin
{
    using Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using Prsd.Core.Mediator;
    using Security;
    using System.Threading.Tasks;

    public class CompleteOrganisationAdminHandler : IRequestHandler<CompleteOrganisationAdmin, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private IOrganisationDetailsDataAccess dataAccess;

        public CompleteOrganisationAdminHandler(IWeeeAuthorization authorization, IOrganisationDetailsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(CompleteOrganisationAdmin message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            Organisation organisation = await dataAccess.FetchOrganisationAsync(message.OrganisationId);

            organisation.OrganisationStatus = OrganisationStatus.Complete;

            await dataAccess.SaveAsync();

            return true;
        }
    }
}
