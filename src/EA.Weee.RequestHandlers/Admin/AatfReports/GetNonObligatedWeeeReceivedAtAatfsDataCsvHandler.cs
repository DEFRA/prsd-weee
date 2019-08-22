namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core;
    using EA.Weee.Requests.Admin.AatfReports;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler : IRequestHandler<GetUkNonObligatedWeeeReceivedAtAatfsDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetUkNonObligatedWeeeReceivedAtAatfsDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetNonObligatedWeeeReceivedAtAatf(request.ComplianceYear, request.AatfName);

            var csvWriter = csvWriterFactory.Create<NonObligatedWeeeReceivedAtAatfData>();
            csvWriter.DefineColumn(@"Year", i => i.Year);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Date submitted (GMT)", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total non-obligated WEEE received (t)", i => i.TotalNonObligatedWeeeReceived);
            csvWriter.DefineColumn(@"Non-obligated WEEE kept / retained by DCFs (t)", i => i.TotalNonObligatedWeeeReceivedFromDcf);
            var fileContent = csvWriter.Write(items);

            var fileNameParameters = string.Empty;

            if (!string.IsNullOrWhiteSpace(request.AatfName))
            {
                fileNameParameters += $"_{request.AatfName}";
            }

            var fileName = $"{request.ComplianceYear}{fileNameParameters}_AATF non-obligated WEEE data_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
