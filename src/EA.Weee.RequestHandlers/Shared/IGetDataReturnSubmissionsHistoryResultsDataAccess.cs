namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.DataReturns;
    public interface IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        Task<List<DataReturnSubmissionsHistoryResult>> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear);
    }
}
