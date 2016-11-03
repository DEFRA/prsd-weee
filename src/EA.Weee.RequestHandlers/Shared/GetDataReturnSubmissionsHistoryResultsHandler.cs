namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using Security;

    public class GetDataReturnSubmissionsHistoryResultsHandler : IRequestHandler<GetDataReturnSubmissionsHistoryResults, DataReturnSubmissionsHistoryResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess;

        public GetDataReturnSubmissionsHistoryResultsHandler(IWeeeAuthorization authorization, IGetDataReturnSubmissionsHistoryResultsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnSubmissionsHistoryResult> HandleAsync(GetDataReturnSubmissionsHistoryResults request)
        {
            authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

            var data = await dataAccess.GetDataReturnSubmissionsHistory(request.SchemeId, request.ComplianceYear,
                request.Ordering, request.IncludeSummaryData);

            return new DataReturnSubmissionsHistoryResult()
            {
                Data = data.ConvertAll(x => (DataReturnSubmissionsHistoryData)x),
                ResultCount = data.Count
            };
        }
    }
}
