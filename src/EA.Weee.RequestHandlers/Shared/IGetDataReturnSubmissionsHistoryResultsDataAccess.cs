namespace EA.Weee.RequestHandlers.Shared
{
    using Core.DataReturns;
    using Requests.Shared;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        Task<DataReturnSubmissionsHistoryResult> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear,
            DataReturnSubmissionsHistoryOrderBy? ordering, bool includeSummaryData);
    }
}
