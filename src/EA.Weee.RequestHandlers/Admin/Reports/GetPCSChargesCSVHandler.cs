namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin; 
    using Security;

    internal class GetPCSChargesCSVHandler : IRequestHandler<GetPCSChargesCSV, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetPCSChargesCSVHandler(IWeeeAuthorization authorization, WeeeContext context, CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetPCSChargesCSV request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(
                       request.ComplianceYear, request.CompetentAuthorityId);

            CsvWriter<PCSChargesCSVData> csvWriter = csvWriterFactory.Create<PCSChargesCSVData>();
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Submission date and time (GMT)", i => i.SubmissionDate.ToString("dd/MM/yyyy HH:mm:ss"));
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Charge value", i => i.ChargeValue);
            csvWriter.DefineColumn(@"Charge band", i => i.ChargeBandType);
            string fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0} - pcschargebreakdown_{1:ddMMyyyy_HHmm}.csv",
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
