namespace EA.Weee.Requests.Admin.Reports
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;

    public class GetSchemesWithObligationOrEvidence : IRequest<List<SchemeData>>
    {
        public int ComplianceYear { get; private set; }

        public GetSchemesWithObligationOrEvidence(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
