namespace EA.Weee.RequestHandlers.Shared
{
    using Core.DataReturns;
    using DataAccess;
    using Requests.Shared;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetDataReturnSubmissionsHistoryResultsDataAccess : IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DataReturnSubmissionsHistoryResult> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear = null,
            DataReturnSubmissionsHistoryOrderBy? ordering = null)
        {
            var results = 
                from dru in context.DataReturnsUploads
                join user in context.Users on dru.DataReturnVersion.SubmittingUserId equals user.Id
                where dru.Scheme.Id == schemeId &&
                dru.DataReturnVersion != null &&
                (!complianceYear.HasValue || dru.ComplianceYear == complianceYear)
                select new DataReturnSubmissionsHistoryData
                {
                    SchemeId = dru.Scheme.Id,
                    OrganisationId = dru.Scheme.OrganisationId,
                    DataReturnUploadId = dru.Id,
                    SubmittedBy = user.FirstName + " " + user.Surname,
                    ComplianceYear = dru.ComplianceYear.Value,
                    SubmissionDateTime = dru.DataReturnVersion.SubmittedDate.Value,
                    FileName = dru.FileName,
                    Quarter = (QuarterType)dru.Quarter
                };

            IOrderedQueryable<DataReturnSubmissionsHistoryData> sortedResults;

            switch (ordering)
            {
                case DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending:
                    sortedResults = results
                        .OrderBy(s => s.ComplianceYear)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending:
                    sortedResults = results
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.QuarterAscending:
                    sortedResults = results
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenBy(s => s.Quarter)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.QuarterDescending:
                    sortedResults = results
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenByDescending(s => s.Quarter)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending:
                    sortedResults = results
                        .OrderBy(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending:
                default:
                    sortedResults = results
                        .OrderByDescending(s => s.SubmissionDateTime);
                    break;
            }

            var dataReturnSubmissionsHistoryResult = new DataReturnSubmissionsHistoryResult();
            dataReturnSubmissionsHistoryResult.Data = await sortedResults.ToListAsync();
            dataReturnSubmissionsHistoryResult.ResultCount = await results.CountAsync();

            return dataReturnSubmissionsHistoryResult;
        }
    }
}
