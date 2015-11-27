namespace EA.Weee.Requests.Shared
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetSubmissionsHistoryResults : IRequest<List<SubmissionsHistorySearchResult>>
    {
        public Guid SchemeId { get; set; }
        public Guid OrganisationId { get; set; }
        public int? ComplianceYear { get; set; }

        public GetSubmissionsHistoryResults(Guid schemeId, Guid organisationId, int? complianceYear = null)
        {
            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}