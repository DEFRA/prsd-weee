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

        public GetDataReturnSubmissionsHistoryResults(Guid schemeId, Guid organisationId, int? complianceYear = null)
        {
            SchemeId = schemeId;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
