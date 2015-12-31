namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using Security;

    public class GetDataReturnSubmissionsHistoryResultsHandler : IRequestHandler<GetDataReturnSubmissionsHistoryResults, List<DataReturnSubmissionsHistoryResult>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess;

        public GetDataReturnSubmissionsHistoryResultsHandler(IWeeeAuthorization authorization, IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<DataReturnSubmissionsHistoryResult>> HandleAsync(GetDataReturnSubmissionsHistoryResults request)
        {
            authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

            return await dataAccess.GetDataReturnSubmissionsHistory(request.SchemeId);
        }
    }
}
