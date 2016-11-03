namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Requests.Shared;

    public interface IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        Task<List<DataReturnSubmissionsData>> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear,
            DataReturnSubmissionsHistoryOrderBy? ordering, bool includeSummaryData);
    }
}
