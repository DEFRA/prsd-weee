namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using Domain.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAatfSubmissionHistoryDataAccess
    {
        Task<List<AatfSubmissionHistory>> GetItemsAsync(Guid aatfId);
    }
}