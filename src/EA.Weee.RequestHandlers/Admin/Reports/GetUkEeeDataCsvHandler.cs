namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetUkEeeDataCsvHandler : IRequestHandler<GetUkEeeDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetUkEeeDataCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetUkEeeDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.SpgUKEEEDataByComplianceYear(
                       request.ComplianceYear);

            CsvWriter<UkEeeCsvData> csvWriter = csvWriterFactory.Create<UkEeeCsvData>();
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total B2B EEE (t)", i => i.TotalB2BEEE);
            csvWriter.DefineColumn(@"Q1 B2B EEE (t)", i => i.Q1B2BEEE);
            csvWriter.DefineColumn(@"Q2 B2B EEE (t)", i => i.Q2B2BEEE);
            csvWriter.DefineColumn(@"Q3 B2B EEE (t)", i => i.Q3B2BEEE);
            csvWriter.DefineColumn(@"Q4 B2B EEE (t)", i => i.Q4B2BEEE);
            csvWriter.DefineColumn(@"Total B2C EEE (t)", i => i.TotalB2CEEE);
            csvWriter.DefineColumn(@"Q1 B2C EEE (t)", i => i.Q1B2CEEE);
            csvWriter.DefineColumn(@"Q2 B2C EEE (t)", i => i.Q2B2CEEE);
            csvWriter.DefineColumn(@"Q3 B2C EEE (t)", i => i.Q3B2CEEE);
            csvWriter.DefineColumn(@"Q4 B2C EEE (t)", i => i.Q4B2CEEE);
            var fileContent = csvWriter.Write(items);
            
            var fileName = string.Format("{0}_UK_EEE_{1:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
