namespace EA.Weee.RequestHandlers.Organisations.GetOrganisationOverview
{
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.Organisations;
    using Security;

    public class GetOrganisationOverviewHandler : IRequestHandler<GetOrganisationOverview, OrganisationOverview>
    {
        private readonly IGetOrganisationOverviewDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetOrganisationOverviewHandler(IWeeeAuthorization authorization, IGetOrganisationOverviewDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<OrganisationOverview> HandleAsync(GetOrganisationOverview message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisationOverview = new OrganisationOverview();
            organisationOverview.HasMemberSubmissions = await dataAccess.HasMemberSubmissions(message.OrganisationId);
            organisationOverview.HasMultipleOrganisationUsers = await dataAccess.HasMultipleManageableOrganisationUsers(message.OrganisationId);
            organisationOverview.HasDataReturnSubmissions = await dataAccess.HasDataReturnSubmissions(message.OrganisationId);

            return organisationOverview;
        }
    }
}
