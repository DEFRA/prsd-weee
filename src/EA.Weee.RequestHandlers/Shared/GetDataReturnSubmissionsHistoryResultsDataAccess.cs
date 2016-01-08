namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using DataAccess;

    public class GetDataReturnSubmissionsHistoryResultsDataAccess : IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<DataReturnSubmissionsHistoryResult>> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear = null)
        {
            var results = await(from dru in context.DataReturnsUploads
                                 join user in context.Users on dru.DataReturnVersion.SubmittingUserId equals user.Id
                                 where dru.Scheme.Id == schemeId && dru.DataReturnVersion != null &&
                                      (!complianceYear.HasValue || dru.ComplianceYear == complianceYear)
                                select new DataReturnSubmissionsHistoryResult
                                 {
                                     SchemeId = dru.Scheme.Id,
                                     OrganisationId = dru.Scheme.OrganisationId,
                                     DataReturnUploadId = dru.Id,
                                     SubmittedBy = user.FirstName + " " + user.Surname,
                                     ComplianceYear = dru.ComplianceYear.Value,
                                     SubmissionDateTime = dru.DataReturnVersion.SubmittedDate.Value,
                                     FileName = dru.FileName,
                                     Quarter = (QuarterType)dru.Quarter,
                                 }).OrderByDescending(s => s.SubmissionDateTime).ToListAsync();
            return results;
        }
    }
}
