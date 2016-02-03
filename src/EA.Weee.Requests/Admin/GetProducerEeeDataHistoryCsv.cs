namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetProducerEeeDataHistoryCsv : IRequest<CSVFileData>
    {
        public string PRN { get; private set; }

        public GetProducerEeeDataHistoryCsv(string prn)
        {
            PRN = prn;
        }
    }
}
