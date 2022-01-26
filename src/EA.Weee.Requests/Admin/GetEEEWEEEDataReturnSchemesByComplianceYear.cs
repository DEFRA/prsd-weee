namespace EA.Weee.Requests.Admin
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class GetEEEWEEEDataReturnSchemesByComplianceYear : IRequest<List<SchemeData>>
    {
        public FilterType Filter { get; private set; }

        public int ComplianceYear { get; set; }

        public GetEEEWEEEDataReturnSchemesByComplianceYear(FilterType filter, int complianceYear)
        {
            Filter = filter;
            ComplianceYear = complianceYear;
        }
    }
}
