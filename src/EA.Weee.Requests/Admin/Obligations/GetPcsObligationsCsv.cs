namespace EA.Weee.Requests.Admin.Obligations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;

    public class GetPcsObligationsCsv : IRequest<CSVFileData>
    {
        public Guid AuthorityId { get; private set; }

        public GetPcsObligationsCsv(Guid authorityId)
        {
            AuthorityId = authorityId;
        }
    }
}
