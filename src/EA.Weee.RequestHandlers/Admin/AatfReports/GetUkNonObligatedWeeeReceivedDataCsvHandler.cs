namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;

    internal class GetUkNonObligatedWeeeReceivedDataCsvHandler : IRequestHandler<GetUkNonObligatedWeeeReceivedDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetUkNonObligatedWeeeReceivedDataCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetUkNonObligatedWeeeReceivedDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetUkNonObligatedWeeeReceivedByComplianceYear(request.ComplianceYear);

            var csvWriter = csvWriterFactory.Create<UkNonObligatedWeeeReceivedData>();
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total non-obligated WEEE received (t)", i => i.TotalNonObligatedWeeeReceived);
            csvWriter.DefineColumn(@"Non-obligated WEEE kept / retained by DCFs (t)", i => i.TotalNonObligatedWeeeReceivedFromDcf);
            var fileContent = csvWriter.Write(items);

            var fileName = $"{request.ComplianceYear}_UK non-obligated WEEE_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
