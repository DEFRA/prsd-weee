namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    internal class GetProducerEeeDataHistoryCsvHandler : IRequestHandler<GetProducerEeeDataHistoryCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerEeeDataHistoryCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducerEeeDataHistoryCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (string.IsNullOrEmpty(request.PRN))
            {
                throw new ArgumentException("PRN is required.");
            }
            var fileName = string.Format("{0}_producerEeehistory_{1:ddMMyyyy_HHmm}.csv",
                request.PRN,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = string.Empty,
                FileName = fileName
            };
        }        
    }
}
