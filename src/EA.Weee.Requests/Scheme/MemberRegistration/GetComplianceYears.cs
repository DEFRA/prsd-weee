namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class GetComplianceYears : IRequest<List<int>>
    {
        public Guid PcsId { get; private set; }

        public GetComplianceYears(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
