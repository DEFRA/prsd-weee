namespace EA.Weee.Requests.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class FetchDataReturnComplianceYearsForScheme : IRequest<List<int>>
    {
        public Guid PcsId { get; private set; }

        public FetchDataReturnComplianceYearsForScheme(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
