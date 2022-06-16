namespace EA.Weee.Requests.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Admin.Obligation;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetSchemeObligation : IRequest<List<SchemeObligationData>>
    {
        public CompetentAuthority Authority { get; }

        public int ComplianceYear { get; }

        public GetSchemeObligation(CompetentAuthority authority, int complianceYear)
        {
            Condition.Requires(complianceYear).IsGreaterThan(0);

            Authority = authority;
            ComplianceYear = complianceYear;
        }
    }
}
