namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetSubmissionsHistoryResultsDataAccess
    {
        Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(Guid schemeId, int? complianceYear);
    }
}
