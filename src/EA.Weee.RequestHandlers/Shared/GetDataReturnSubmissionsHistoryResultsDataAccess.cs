namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Obligation;
    using Requests.Shared;

    public class GetDataReturnSubmissionsHistoryResultsDataAccess : IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<DataReturnSubmissionsData>> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear = null,
            DataReturnSubmissionsHistoryOrderBy? ordering = null, bool includeSummaryData = false)
        {
            var query =
                from dru in context.DataReturnsUploads
                join user in context.Users on dru.DataReturnVersion.SubmittingUserId equals user.Id
                where dru.Scheme.Id == schemeId &&
                dru.DataReturnVersion != null &&
                (!complianceYear.HasValue || dru.ComplianceYear == complianceYear)
                select new DataReturnSubmissionsData
                {
                    SchemeId = dru.Scheme.Id,
                    OrganisationId = dru.Scheme.OrganisationId,
                    DataReturnUploadId = dru.Id,
                    SubmittedBy = user.FirstName + " " + user.Surname,
                    ComplianceYear = dru.ComplianceYear.Value,
                    SubmissionDateTime = dru.DataReturnVersion.SubmittedDate.Value,
                    FileName = dru.FileName,
                    Quarter = (Core.DataReturns.QuarterType)dru.Quarter,
                    DataReturnVersion = dru.DataReturnVersion
                };

            switch (ordering)
            {
                case DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending:
                    query = query
                        .OrderBy(s => s.ComplianceYear)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending:
                    query = query
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.QuarterAscending:
                    query = query
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenBy(s => s.Quarter)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.QuarterDescending:
                    query = query
                        .OrderByDescending(s => s.ComplianceYear)
                        .ThenByDescending(s => s.Quarter)
                        .ThenByDescending(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending:
                    query = query
                        .OrderBy(s => s.SubmissionDateTime);
                    break;

                case DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending:
                default:
                    query = query
                        .OrderByDescending(s => s.SubmissionDateTime);
                    break;
            }

            var results = await query.ToListAsync();

            if (includeSummaryData)
            {
                foreach (var result in results)
                {
                    var returnVersion = result.DataReturnVersion;

                    result.EeeOutputB2b =
                        returnVersion.EeeOutputReturnVersion != null ?
                        CalculateTonnage(returnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                           ObligationType.B2B)
                        : null;

                    result.EeeOutputB2c =
                        returnVersion.EeeOutputReturnVersion != null ?
                        CalculateTonnage(returnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                        ObligationType.B2C)
                        : null;

                    result.WeeeCollectedB2b =
                        returnVersion.WeeeCollectedReturnVersion != null ?
                        CalculateTonnage(returnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts,
                        ObligationType.B2B)
                        : null;

                    result.WeeeCollectedB2c =
                        returnVersion.WeeeCollectedReturnVersion != null ?
                        CalculateTonnage(returnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts,
                        ObligationType.B2C)
                        : null;

                    result.WeeeDeliveredB2b =
                        returnVersion.WeeeDeliveredReturnVersion != null ?
                        CalculateTonnage(returnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                        ObligationType.B2B)
                        : null;

                    result.WeeeDeliveredB2c =
                        returnVersion.WeeeDeliveredReturnVersion != null ?
                        CalculateTonnage(returnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                        ObligationType.B2C)
                        : null;
                }
            }

            return results;
        }

        private decimal? CalculateTonnage(IEnumerable<ReturnItem> returnItems, ObligationType obligationType)
        {
            var filteredReturnItems =
                returnItems.Where(r => r.ObligationType == obligationType);

            return filteredReturnItems.Any() ?
                (decimal?)filteredReturnItems.Sum(r => r.Tonnage)
                : null;
        }
    }
}
