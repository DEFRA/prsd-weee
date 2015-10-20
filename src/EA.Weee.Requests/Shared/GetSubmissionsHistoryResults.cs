namespace EA.Weee.Requests.Shared
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSubmissionsHistoryResults : IRequest<List<SubmissionsHistorySearchResult>>
    {
        public Guid SchemeId { get; set; }
        public Guid OrganisationId { get; set; }
        public int ComplianceYear { get; set; }

        public GetSubmissionsHistoryResults(Guid id, int year = 0, Guid orgId = default(Guid))
        {
            ComplianceYear = year;
            SchemeId = id;
            OrganisationId = orgId;
        }
    }
}