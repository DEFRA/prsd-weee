namespace EA.Weee.RequestHandlers.Admin.Reports.GetSchemeObligationDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetSchemeObligationDataCsvHandler : IRequestHandler<GetSchemeObligationDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemeObligationCsvDataProcessor dataProcessor;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetSchemeObligationDataCsvHandler(
            IWeeeAuthorization authorization,
            IGetSchemeObligationCsvDataProcessor dataProcessor,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataProcessor = dataProcessor;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetSchemeObligationDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            CsvWriter<SchemeObligationCsvData> csvWriter = CreateWriter();

            List<SchemeObligationCsvData> items = await dataProcessor.FetchObligationsForComplianceYearAsync(request.ComplianceYear);

            string fileContent = csvWriter.Write(items);

            string fileName = string.Format("{0}_pcsobligationdata_{1:ddMMyyyy_HHmm}.csv",
                    request.ComplianceYear,
                    DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        public CsvWriter<SchemeObligationCsvData> CreateWriter()
        {
            CsvWriter<SchemeObligationCsvData> csvWriter = csvWriterFactory.Create<SchemeObligationCsvData>();

            csvWriter.DefineColumn(@"Scheme approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Obligation type previous compliance year", i => i.ObligationTypeForPreviousYear);
            csvWriter.DefineColumn(@"Obligation type compliance year selected", i => i.ObligationTypeForSelectedYear);

            foreach (int category in Enumerable.Range(1, 14))
            {
                string title = string.Format("Cat{0} B2C (t)", category);
                string columnName = string.Format("Cat{0}B2CTotal", category);
                csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));
            }

            return csvWriter;
        }
    }
}
