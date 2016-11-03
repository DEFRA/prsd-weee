namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Linq;
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

            var result = await dataAccess.GetDataReturnSubmissionsHistory(request.SchemeId, request.ComplianceYear,
                request.Ordering, request.IncludeSummaryData);

            var data = result.Select(x =>
            new DataReturnSubmissionsHistoryData
            {
                SchemeId = x.SchemeId,
                OrganisationId = x.OrganisationId,
                DataReturnUploadId = x.DataReturnUploadId,
                SubmittedBy = x.SubmittedBy,
                ComplianceYear = x.ComplianceYear,
                SubmissionDateTime = x.SubmissionDateTime,
                FileName = x.FileName,
                Quarter = x.Quarter,
                EeeOutputB2b = x.EeeOutputB2b,
                EeeOutputB2c = x.EeeOutputB2c,
                WeeeCollectedB2b = x.WeeeCollectedB2b,
                WeeeCollectedB2c = x.WeeeCollectedB2c,
                WeeeDeliveredB2b = x.WeeeDeliveredB2b,
                WeeeDeliveredB2c = x.WeeeDeliveredB2c,
                DataReturnVersionId = x.DataReturnVersion.Id
            })
            .ToList();

            return new DataReturnSubmissionsHistoryResult()
            {
                Data = data,
                ResultCount = data.Count
            };
        }
    }
}
