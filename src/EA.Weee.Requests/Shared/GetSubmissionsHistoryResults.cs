namespace EA.Weee.Requests.Shared
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSubmissionsHistoryResults : IRequest<SubmissionsHistorySearchResult>
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
        public SubmissionsHistoryOrderBy? Ordering { get; set; }

        public GetSubmissionsHistoryResults(Guid schemeId, Guid organisationId, int? complianceYear = null)
        {
            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}