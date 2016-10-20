namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using Requests.Shared;

    public class GetSubmissionsHistoryResultsDataAccess : IGetSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<SubmissionsHistorySearchResult> GetSubmissionsHistory(Guid schemeId, int? complianceYear = null,
            SubmissionsHistoryOrderBy? ordering = null)
        {
            var results =
                from mu in context.MemberUploads
                where mu.IsSubmitted &&
                mu.Scheme.Id == schemeId &&
                (!complianceYear.HasValue || mu.ComplianceYear == complianceYear)
                select new SubmissionsHistorySearchData
                {
                    SchemeId = mu.Scheme.Id,
                    OrganisationId = mu.OrganisationId,
                    MemberUploadId = mu.Id,
                    SubmittedBy = mu.SubmittedByUser.FirstName + " " + mu.SubmittedByUser.Surname,
                    Year = mu.ComplianceYear.Value,
                    DateTime = mu.SubmittedDate.Value,
                    TotalCharges = mu.TotalCharges,
                    NoOfWarnings = (from me in context.MemberUploadErrors
                                    where me.MemberUploadId == mu.Id &&
                                    (me.ErrorLevel.Value == Domain.Error.ErrorLevel.Warning.Value)
                                    select me).Count(),
                    FileName = mu.FileName
                };

            IOrderedQueryable<SubmissionsHistorySearchData> sortedResults;

            switch (ordering)
            {
                case SubmissionsHistoryOrderBy.ComplianceYearAscending:
                    sortedResults = results
                        .OrderBy(r => r.Year)
                        .ThenByDescending(r => r.DateTime);
                    break;

                case SubmissionsHistoryOrderBy.ComplianceYearDescending:
                    sortedResults = results
                        .OrderByDescending(r => r.Year)
                        .ThenByDescending(r => r.DateTime);
                    break;

                case SubmissionsHistoryOrderBy.SubmissionDateAscending:
                    sortedResults = results
                        .OrderBy(r => r.DateTime);
                    break;

                case SubmissionsHistoryOrderBy.SubmissionDateDescending:
                default:
                    sortedResults = results
                        .OrderByDescending(r => r.DateTime);
                    break;
            }

            var submissionsHistorySearchResult = new SubmissionsHistorySearchResult();
            submissionsHistorySearchResult.Data = await sortedResults.ToListAsync();
            submissionsHistorySearchResult.ResultCount = await results.CountAsync();

            return submissionsHistorySearchResult;
        }
    }
}
