namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSubmissionsHistoryResults : IRequest<List<SubmissionsHistorySearchResult>>
    {
        public Guid SchemeId { get; set; }
        public int ComplianceYear { get; set; }

        public GetSubmissionsHistoryResults(int year, Guid id)
        {
            ComplianceYear = year;
            SchemeId = id;
        }
    }
}
