namespace EA.Weee.Requests.Scheme
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSubmissionsHistoryResults : IRequest<List<SubmissionsHistorySearchResult>>
    {
        public Guid OrganisationId { get; set; }

        public GetSubmissionsHistoryResults(Guid orgId)
        {
            OrganisationId = orgId;
        }
    }
}