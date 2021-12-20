namespace EA.Weee.Requests.Admin
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetMemberRegisteredSchemesByComplianceYear : IRequest<List<SchemeData>>
    {
        public enum FilterType
        {
            Approved,
            ApprovedOrWithdrawn
        }

        public FilterType Filter { get; private set; }

        public int ComplianceYear { get; set; }

        public GetMemberRegisteredSchemesByComplianceYear(FilterType filter, int complianceYear)
        {
            Filter = filter;
            ComplianceYear = complianceYear;
        }
    }
}
