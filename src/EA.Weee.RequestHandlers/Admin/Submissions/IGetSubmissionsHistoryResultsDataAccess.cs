namespace EA.Weee.RequestHandlers.Admin.Submissions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetSubmissionsHistoryResultsDataAccess
    {
        Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(int year, Guid schemeId);
    }
}
