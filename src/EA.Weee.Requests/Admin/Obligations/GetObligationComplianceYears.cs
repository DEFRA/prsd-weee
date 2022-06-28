namespace EA.Weee.Requests.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetObligationComplianceYears : IRequest<List<int>>
    {
        public CompetentAuthority Authority { get; }

        public GetObligationComplianceYears(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
