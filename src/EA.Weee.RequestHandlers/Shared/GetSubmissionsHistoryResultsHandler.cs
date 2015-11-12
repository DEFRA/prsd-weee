namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using Security;

    public class GetSubmissionsHistoryResultsHandler : IRequestHandler<GetSubmissionsHistoryResults, List<SubmissionsHistorySearchResult>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSubmissionsHistoryResultsDataAccess dataAccess;

        public GetSubmissionsHistoryResultsHandler(IWeeeAuthorization authorization, IGetSubmissionsHistoryResultsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<SubmissionsHistorySearchResult>> HandleAsync(GetSubmissionsHistoryResults request)
        {
            authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

            return await dataAccess.GetSubmissionsHistory(request.SchemeId, request.ComplianceYear);
        }
    }
}
