namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;
using System.Collections.Generic;

    public class GetComplianceYears : IRequest<List<int>>
    {
        public Guid PcsId { get; private set; }

        public GetComplianceYears(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
