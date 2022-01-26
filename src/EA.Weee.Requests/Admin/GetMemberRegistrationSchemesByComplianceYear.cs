namespace EA.Weee.Requests.Admin
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class GetMemberRegistrationSchemesByComplianceYear : IRequest<List<SchemeData>>
    {
        public FilterType Filter { get; private set; }

        public int ComplianceYear { get; set; }

        public GetMemberRegistrationSchemesByComplianceYear(FilterType filter, int complianceYear)
        {
            Filter = filter;
            ComplianceYear = complianceYear;
        }
    }
}
