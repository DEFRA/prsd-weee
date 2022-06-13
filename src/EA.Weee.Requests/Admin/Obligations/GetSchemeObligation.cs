namespace EA.Weee.Requests.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Admin.Obligation;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetSchemeObligation : IRequest<List<SchemeObligationData>>
    {
        public CompetentAuthority Authority { get; }

        public int ComplianceYear { get; }

        public GetSchemeObligation(CompetentAuthority authority, int complianceYear)
        {
            Authority = authority;
            ComplianceYear = complianceYear;
        }
    }
}
