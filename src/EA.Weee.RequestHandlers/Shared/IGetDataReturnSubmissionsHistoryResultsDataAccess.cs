﻿namespace EA.Weee.RequestHandlers.Shared
{
    using Requests.Shared;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        Task<List<DataReturnSubmissionsData>> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear,
            DataReturnSubmissionsHistoryOrderBy? ordering, bool includeSummaryData);
    }
}
