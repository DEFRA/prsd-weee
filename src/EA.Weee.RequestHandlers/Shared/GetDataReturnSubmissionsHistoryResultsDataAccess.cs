namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
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
                from dru in context.DataReturnsUploads.AsNoTracking()
                join user in context.Users.AsNoTracking() on dru.DataReturnVersion.SubmittingUserId equals user.Id
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
                    DataReturnVersion = dru.DataReturnVersion,

                    EeeOutputB2b = includeSummaryData && dru.DataReturnVersion.EeeOutputReturnVersion != null ?
                            dru.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2B) ?
                               (decimal?)dru.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2B).Sum(z => z.Tonnage)
                               : null
                             : null,

                    EeeOutputB2c = includeSummaryData && dru.DataReturnVersion.EeeOutputReturnVersion != null ?
                            dru.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2C) ?
                               (decimal?)dru.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2C).Sum(z => z.Tonnage)
                               : null
                             : null,

                    WeeeCollectedB2b = includeSummaryData && dru.DataReturnVersion.WeeeCollectedReturnVersion != null ?
                            dru.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2B) ?
                               (decimal?)dru.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2B).Sum(z => z.Tonnage)
                               : null
                             : null,

                    WeeeCollectedB2c = includeSummaryData && dru.DataReturnVersion.WeeeCollectedReturnVersion != null ?
                            dru.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2C) ?
                               (decimal?)dru.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2C).Sum(z => z.Tonnage)
                               : null
                             : null,

                    WeeeDeliveredB2b = includeSummaryData && dru.DataReturnVersion.WeeeDeliveredReturnVersion != null ?
                            dru.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2B) ?
                               (decimal?)dru.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2B).Sum(z => z.Tonnage)
                               : null
                             : null,

                    WeeeDeliveredB2c = includeSummaryData && dru.DataReturnVersion.WeeeDeliveredReturnVersion != null ?
                            dru.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Any(x => x.DatabaseObligationType == ObligationTypeString.B2C) ?
                               (decimal?)dru.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts
                                  .Where(y => y.DatabaseObligationType == ObligationTypeString.B2C).Sum(z => z.Tonnage)
                               : null
                             : null,
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

            return await query.ToListAsync();
        }
    }
}
