namespace EA.Weee.RequestHandlers.Shared
{
    using System;
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
            SubmissionsHistoryOrderBy? ordering = null, bool includeSummaryData = false)
        {
            var results =
                from mu in context.MemberUploads
                where mu.IsSubmitted &&
                mu.Scheme.Id == schemeId &&
                (!complianceYear.HasValue || mu.ComplianceYear == complianceYear)
                let submissionProducers = context.AllProducerSubmissions // Producers associated with the submission
                                            .Where(s => s.MemberUploadId == mu.Id)
                                            .Select(s => s.RegisteredProducer.ProducerRegistrationNumber)
                let submissionProducersCount = submissionProducers.Count()
                let schemeNonRemovedProducers = context.ProducerSubmissions // Non-removed producers registered with the scheme prior to the submission
                                            .Where(s => s.RegisteredProducer.Scheme.Id == schemeId)
                                            .Where(s => s.RegisteredProducer.ComplianceYear == mu.ComplianceYear)
                                            .Where(s => s.MemberUpload.IsSubmitted)
                                            .Where(s => s.MemberUpload.SubmittedDate < mu.SubmittedDate)
                                            .Select(s => s.RegisteredProducer.ProducerRegistrationNumber)
                let schemeRemovedProducers = context.RemovedProducerSubmissions // Producers submitted prior to the current submission but removed after the submission
                                            .Where(s => s.RegisteredProducer.Scheme.Id == schemeId)
                                            .Where(s => s.RegisteredProducer.ComplianceYear == mu.ComplianceYear)
                                            .Where(s => s.MemberUpload.IsSubmitted)
                                            .Where(s => s.MemberUpload.SubmittedDate < mu.SubmittedDate)
                                            .Where(s => s.RegisteredProducer.RemovedDate > mu.SubmittedDate)
                                            .Select(s => s.RegisteredProducer.ProducerRegistrationNumber)
                let schemeProducers = schemeRemovedProducers
                                            .Union(schemeNonRemovedProducers)
                let producerAmendmentsCount = submissionProducers // Producers associated with the submission and having records prior to the current submission are classed as amendments
                                            .Intersect(schemeProducers)
                                            .Count()
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
                    FileName = mu.FileName,
                    NumberOfChangedProducers = includeSummaryData ? (int?)producerAmendmentsCount : null,
                    NumberOfNewProducers = includeSummaryData ? (int?)(submissionProducersCount - producerAmendmentsCount) : null,
                    ProducersChanged = includeSummaryData ? (bool?)(submissionProducersCount > 0) : null
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
