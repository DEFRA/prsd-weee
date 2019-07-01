namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;

    public class GetAatfSubmissionHistoryDataAccess : IGetAatfSubmissionHistoryDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfSubmissionHistoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<AatfSubmissionHistory>> GetItemsAsync(Guid aatfId)
        {
            return await context.StoredProcedures.GetAatfSubmissions(aatfId);
        }
    }
}
