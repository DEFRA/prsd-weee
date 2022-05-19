namespace EA.Weee.Requests.Admin.Obligations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;

    public class GetPcsObligationsCsv : IRequest<CSVFileData>
    {
        public CompetentAuthority Authority { get; private set; }

        public GetPcsObligationsCsv(CompetentAuthority authority)
        {
            Authority = authority;
        }
    }
}
