namespace EA.Weee.RequestHandlers.Shared
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using DataReturns;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using Security;

    public class GetDataReturnSubmissionsHistoryResultsHandler : IRequestHandler<GetDataReturnSubmissionsHistoryResults, DataReturnSubmissionsHistoryResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnSubmissionsHistoryResultsDataAccess historyResultsDataAccess;
        private readonly IDataReturnSubmissionsDataAccess submissionsDataAccess;
        private readonly IDataReturnVersionComparer dataReturnVersionComparer;

        public GetDataReturnSubmissionsHistoryResultsHandler(
            IWeeeAuthorization authorization,
            IGetDataReturnSubmissionsHistoryResultsDataAccess historyResultsDataAccess,
            IDataReturnSubmissionsDataAccess submissionsDataAccess,
            IDataReturnVersionComparer dataReturnVersionComparer)
        {
            this.authorization = authorization;
            this.historyResultsDataAccess = historyResultsDataAccess;
            this.submissionsDataAccess = submissionsDataAccess;
            this.dataReturnVersionComparer = dataReturnVersionComparer;
        }

        public async Task<DataReturnSubmissionsHistoryResult> HandleAsync(GetDataReturnSubmissionsHistoryResults request)
        {
            authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

            var historyResult = await historyResultsDataAccess.GetDataReturnSubmissionsHistory(request.SchemeId, request.ComplianceYear,
                request.Ordering, request.IncludeSummaryData);

            var historyData = historyResult.Select(x =>
            new
            {
                Result = x,
                HistoryData =
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
                   }
            })
            .ToList();

            if (request.CompareEeeOutputData)
            {
                foreach (var item in historyData)
                {
                    var currentDataVersion = item.Result.DataReturnVersion;
                    var previousSubmission = await submissionsDataAccess.GetPreviousSubmission(currentDataVersion);

                    var eeeDataChanged = dataReturnVersionComparer.EeeDataChanged(currentDataVersion, previousSubmission);

                    item.HistoryData.EeeDataChanged = eeeDataChanged;
                    if (eeeDataChanged)
                    {
                        item.HistoryData.PreviousSubmissionDataReturnVersionId = previousSubmission.Id;
                    }
                }
            }

            var historyDataList = historyData
                .Select(x => x.HistoryData)
                .ToList();

            return new DataReturnSubmissionsHistoryResult()
            {
                Data = historyDataList,
                ResultCount = historyDataList.Count
            };
        }
    }
}
