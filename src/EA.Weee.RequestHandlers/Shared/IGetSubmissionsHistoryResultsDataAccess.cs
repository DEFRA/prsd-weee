namespace EA.Weee.RequestHandlers.Shared
{
    using Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSubmissionsHistoryResultsDataAccess
    {
        Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(Guid schemeId, int? complianceYear);
    }
}
