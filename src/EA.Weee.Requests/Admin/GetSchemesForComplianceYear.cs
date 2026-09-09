namespace EA.Weee.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Shared;
    using EA.Weee.Core.Scheme;
    using Prsd.Core.Mediator;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class GetSchemesForComplianceYear : IRequest<List<SchemeData>>
    {
        public int? ComplianceYear { get; set; }

        public GetSchemesForComplianceYear(int? complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
