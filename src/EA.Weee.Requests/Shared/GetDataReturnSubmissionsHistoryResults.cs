namespace EA.Weee.Requests.Shared
{
    using System;
    using System.Collections.Generic;
    using Core.DataReturns;
    using Prsd.Core.Mediator;

    public class GetDataReturnSubmissionsHistoryResults : IRequest<List<DataReturnSubmissionsHistoryResult>>
    {
        public Guid SchemeId { get; set; }
        public Guid OrganisationId { get; set; }
        public int? ComplianceYear { get; set; }

        public GetDataReturnSubmissionsHistoryResults(Guid schemeId, Guid organisationId, int? complianceYear = null)
        {
            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
