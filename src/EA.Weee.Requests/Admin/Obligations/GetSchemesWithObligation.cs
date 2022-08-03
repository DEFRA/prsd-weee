namespace EA.Weee.Requests.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemesWithObligation : IRequest<List<SchemeData>>
    {
        public int ComplianceYear { get; }

        public GetSchemesWithObligation(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
