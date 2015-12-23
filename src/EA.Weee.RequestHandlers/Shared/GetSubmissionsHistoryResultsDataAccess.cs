namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;

    public class GetSubmissionsHistoryResultsDataAccess : IGetSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(Guid schemeId, int? complianceYear = null)
        {
            var results = await(from mu in context.MemberUploads
                                join user in context.Users on mu.CreatedById equals user.Id
                                where mu.IsSubmitted &&
                                      mu.Scheme.Id == schemeId &&
                                      (!complianceYear.HasValue || mu.ComplianceYear == complianceYear)
                                 select new SubmissionsHistorySearchResult
                                 {
                                     SchemeId = mu.Scheme.Id,
                                     OrganisationId = mu.OrganisationId,
                                     MemberUploadId = mu.Id,
                                     SubmittedBy = user.FirstName + " " + user.Surname,
                                     Year = mu.ComplianceYear.Value,
                                     DateTime = mu.CreatedDate,
                                     TotalCharges = mu.TotalCharges,
                                     NoOfWarnings = (from me in context.MemberUploadErrors
                                                     where me.MemberUploadId == mu.Id && (me.ErrorLevel.Value == Domain.ErrorLevel.Warning.Value)
                                                     select me).Count(),
                                     FileName = mu.FileName
                                 }).OrderByDescending(s => s.DateTime).ToListAsync();
            return results;
        }
    }
}
