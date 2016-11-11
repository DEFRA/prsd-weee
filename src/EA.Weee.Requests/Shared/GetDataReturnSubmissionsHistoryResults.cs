namespace EA.Weee.Requests.Shared
{
    using Core.DataReturns;
    using Prsd.Core.Mediator;
    using System;

    public class GetDataReturnSubmissionsHistoryResults : IRequest<DataReturnSubmissionsHistoryResult>
    {
        public Guid SchemeId { get; set; }
        public Guid OrganisationId { get; set; }
        public int? ComplianceYear { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// The sort order. A nullable value is required due to sorting
        /// not implemented for internal users.
        /// </summary>
        public DataReturnSubmissionsHistoryOrderBy? Ordering { get; set; }

        public bool IncludeSummaryData { get; set; }

        public bool CompareEeeOutputData { get; set; }

        public GetDataReturnSubmissionsHistoryResults(Guid schemeId, Guid organisationId, int? complianceYear = null,
            DataReturnSubmissionsHistoryOrderBy? ordering = null, bool includeSummaryData = false,
            bool compareEeeOutputData = false)
        {
            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
            Ordering = ordering;
            IncludeSummaryData = includeSummaryData;
            CompareEeeOutputData = compareEeeOutputData;
        }
    }
}
