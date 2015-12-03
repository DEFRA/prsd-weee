namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetProducerAmendmentsHistoryCSV : IRequest<CSVFileData>
    {
        public string PRN { get; private set; }

        public GetProducerAmendmentsHistoryCSV(string prn)
        {
            PRN = prn;
        }
    }
}
