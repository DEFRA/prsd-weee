namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRunCsv
{
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Charges;
    using Security;

    public class FetchInvoiceRunCsvHandler : IRequestHandler<FetchInvoiceRunCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public FetchInvoiceRunCsvHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            ICommonDataAccess commonDataAccess,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.commonDataAccess = commonDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(FetchInvoiceRunCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            var items = await context.StoredProcedures.SpgInvoiceRunChargeBreakdown(message.InvoiceRunId);

            var csvWriter = csvWriterFactory.Create<PCSChargesCSVData>();
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Submission date and time (GMT)", i => i.SubmissionDate.ToString("dd/MM/yyyy HH:mm:ss"));
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Charge value (£)", i => i.ChargeValue);
            csvWriter.DefineColumn(@"Charge band", i => i.ChargeBandType);

            string fileContent = csvWriter.Write(items);

            var invoiceRun = await commonDataAccess.FetchInvoiceRunAsync(message.InvoiceRunId);
            string fileName = string.Format("invoicerun_{0}_{1}.csv", invoiceRun.CompetentAuthority.Abbreviation, invoiceRun.IssuedDate.ToString("ddMMyyyy"));

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
