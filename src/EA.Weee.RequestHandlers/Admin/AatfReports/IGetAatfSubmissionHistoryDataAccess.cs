namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using Domain.Admin.AatfReports;

    public interface IGetAatfSubmissionHistoryDataAccess
    {
        Task<List<AatfSubmissionHistory>> GetItemsAsync(Guid aatfId);
    }
}