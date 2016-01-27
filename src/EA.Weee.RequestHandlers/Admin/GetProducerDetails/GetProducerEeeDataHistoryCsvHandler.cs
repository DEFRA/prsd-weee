namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
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

            string fileContent = await GetProducersEeeHistoryCsvContent(request.PRN);

            var fileName = string.Format("{0}_producerEeehistory_{1:ddMMyyyy_HHmm}.csv",
                request.PRN,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        private async Task<string> GetProducersEeeHistoryCsvContent(string prn)
        {
            var items = await context.StoredProcedures.SpgProducerEeeHistoryCsvData(prn);

            CsvWriter<ProducerEeeHistoryCsvData> csvWriter =
                csvWriterFactory.Create<ProducerEeeHistoryCsvData>();

            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Scheme approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Compliance year", i => i.Year);
            csvWriter.DefineColumn(@"Date and time (GMT) data submitted", i => i.DateSumbitted);
            csvWriter.DefineColumn(@"Quarter", i => i.QuarterType);
            csvWriter.DefineColumn(@"Latest submission(Yes/No)", i => i.LatestSubmission);
            foreach (int category in Enumerable.Range(1, 14))
            {
                    string title = string.Format("Cat{0} B2C", category);
                    string columnName = string.Format("Cat{0}B2C", category);
                    csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));               
            }
            foreach (int category in Enumerable.Range(1, 14))
            {
                string title = string.Format("Cat{0} B2B", category);
                string columnName = string.Format("Cat{0}B2B", category);
                csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));
            }
            string fileContent = csvWriter.Write(items);
            return fileContent;
        }
    }
}
