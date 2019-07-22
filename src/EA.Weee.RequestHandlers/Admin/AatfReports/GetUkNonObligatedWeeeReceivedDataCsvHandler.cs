namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using EA.Weee.Requests.Admin.Aatf;
    using Prsd.Core.Mediator;
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
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            List<UkNonObligatedWeeeReceivedData> items = await context.StoredProcedures.UKUkNonObligatedWeeeReceivedByComplianceYear(
                       request.ComplianceYear);

            CsvWriter<UkNonObligatedWeeeReceivedData> csvWriter = csvWriterFactory.Create<UkNonObligatedWeeeReceivedData>();
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total non-obligated WEEE received (total tonnes)", i => i.TotalNonObligatedWeeeReceived);
            csvWriter.DefineColumn(@"Non-obligated WEEE received from DCFs (total tonnes)", i => i.TotalNonObligatedWeeeReceivedFromDcf);
            string fileContent = csvWriter.Write(items);

            string fileName = string.Format("{0}_UK_NON_OBLIGATED_WEEE_{1:ddMMyyyy_HHmm}.csv",
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
