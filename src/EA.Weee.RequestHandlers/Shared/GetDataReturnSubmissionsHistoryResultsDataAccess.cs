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
    using Domain.Obligation;

    public class GetDataReturnSubmissionsHistoryResultsDataAccess : IGetDataReturnSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DataReturnSubmissionsHistoryResult> GetDataReturnSubmissionsHistory(Guid schemeId, int? complianceYear = null,
            DataReturnSubmissionsHistoryOrderBy? ordering = null, bool includeSummaryData = false)
        {
            var query =
                from dru in context.DataReturnsUploads
                join user in context.Users on dru.DataReturnVersion.SubmittingUserId equals user.Id
                where dru.Scheme.Id == schemeId &&
                dru.DataReturnVersion != null &&
                (!complianceYear.HasValue || dru.ComplianceYear == complianceYear)
                select new
                {
                    SchemeId = dru.Scheme.Id,
                    OrganisationId = dru.Scheme.OrganisationId,
                    DataReturnUploadId = dru.Id,
                    SubmittedBy = user.FirstName + " " + user.Surname,
                    ComplianceYear = dru.ComplianceYear.Value,
                    SubmissionDateTime = dru.DataReturnVersion.SubmittedDate.Value,
                    FileName = dru.FileName,
                    Quarter = (QuarterType)dru.Quarter,
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

            List<DataReturnSubmissionsHistoryData> historyData;

            if (includeSummaryData)
            {
                historyData = results.Select(x =>
                new DataReturnSubmissionsHistoryData
                {
                    SchemeId = x.SchemeId,
                    OrganisationId = x.OrganisationId,
                    DataReturnUploadId = x.DataReturnUploadId,
                    SubmittedBy = x.SubmittedBy,
                    ComplianceYear = x.ComplianceYear,
                    SubmissionDateTime = x.SubmissionDateTime,
                    FileName = x.FileName,
                    Quarter = x.Quarter,
                    EeeOutputB2b = x.DataReturnVersion.EeeOutputReturnVersion != null ?
                                      (decimal?)x.DataReturnVersion
                                         .EeeOutputReturnVersion
                                         .EeeOutputAmounts
                                         .Where(e => e.ObligationType == ObligationType.B2B)
                                         .Sum(e => e.Tonnage)
                                      : null,
                    EeeOutputB2c = x.DataReturnVersion.EeeOutputReturnVersion != null ?
                                       (decimal?)x.DataReturnVersion
                                          .EeeOutputReturnVersion
                                          .EeeOutputAmounts
                                          .Where(e => e.ObligationType == ObligationType.B2C)
                                          .Sum(e => e.Tonnage)
                                       : null,
                    WeeeCollectedB2b = x.DataReturnVersion.WeeeCollectedReturnVersion != null ?
                                          (decimal?)x.DataReturnVersion
                                             .WeeeCollectedReturnVersion
                                             .WeeeCollectedAmounts
                                             .Where(c => c.ObligationType == ObligationType.B2B)
                                             .Sum(c => c.Tonnage)
                                          : null,
                    WeeeCollectedB2c = x.DataReturnVersion.WeeeCollectedReturnVersion != null ?
                                          (decimal?)x.DataReturnVersion
                                             .WeeeCollectedReturnVersion
                                             .WeeeCollectedAmounts
                                             .Where(c => c.ObligationType == ObligationType.B2C)
                                             .Sum(c => c.Tonnage)
                                          : null,
                    WeeeDeliveredB2b = x.DataReturnVersion.WeeeDeliveredReturnVersion != null ?
                                          (decimal?)x.DataReturnVersion
                                             .WeeeDeliveredReturnVersion
                                             .WeeeDeliveredAmounts
                                             .Where(d => d.ObligationType == ObligationType.B2B)
                                             .Sum(d => d.Tonnage)
                                          : null,
                    WeeeDeliveredB2c = x.DataReturnVersion.WeeeDeliveredReturnVersion != null ?
                                          (decimal?)x.DataReturnVersion
                                             .WeeeDeliveredReturnVersion
                                             .WeeeDeliveredAmounts
                                             .Where(d => d.ObligationType == ObligationType.B2C)
                                             .Sum(d => d.Tonnage)
                                          : null
                })
                .ToList();
            }
            else
            {
                historyData = results.Select(x =>
                new DataReturnSubmissionsHistoryData
                {
                    SchemeId = x.SchemeId,
                    OrganisationId = x.OrganisationId,
                    DataReturnUploadId = x.DataReturnUploadId,
                    SubmittedBy = x.SubmittedBy,
                    ComplianceYear = x.ComplianceYear,
                    SubmissionDateTime = x.SubmissionDateTime,
                    FileName = x.FileName,
                    Quarter = x.Quarter
                })
                .ToList();
            }

            var dataReturnSubmissionsHistoryResult = new DataReturnSubmissionsHistoryResult();
            dataReturnSubmissionsHistoryResult.Data = historyData;
            dataReturnSubmissionsHistoryResult.ResultCount = results.Count;

            return dataReturnSubmissionsHistoryResult;
        }
    }
}
