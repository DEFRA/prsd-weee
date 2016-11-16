namespace EA.Weee.RequestHandlers.Shared
{
    using Core.Admin;
    using Requests.Shared;
    using System;
    using System.Threading.Tasks;

    public interface IGetSubmissionsHistoryResultsDataAccess
    {
        Task<SubmissionsHistorySearchResult> GetSubmissionsHistory(Guid schemeId, int? complianceYear,
            SubmissionsHistoryOrderBy? ordering, bool includeSummaryData);
    }
}
