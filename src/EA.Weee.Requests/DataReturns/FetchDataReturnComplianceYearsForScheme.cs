namespace EA.Weee.Requests.DataReturns
{
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class FetchDataReturnComplianceYearsForScheme : IRequest<List<int>>
    {
        public Guid PcsId { get; private set; }

        public FetchDataReturnComplianceYearsForScheme(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
