namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetSubmissionsHistoryResultsDataAccess
    {
        Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(Guid schemeId, int complianceYear);
    }
}
