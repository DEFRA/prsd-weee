namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using Shared;

    internal class GetPcsAatfComparisonDataHandler : IRequestHandler<GetPcsAatfComparisonData, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetPcsAatfComparisonDataHandler(IWeeeAuthorization authorization,
           WeeeContext context,
           CsvWriterFactory csvWriterFactory,
           ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetPcsAatfComparisonData request)
        {
            authorization.EnsureCanAccessInternalArea();

            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetPcsAatfComparisonDataCsvData(request.ComplianceYear, request.Quarter, request.ObligationType);

            var csvWriter = csvWriterFactory.Create<PcsAatfComparisonDataCsvData>();
            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Quarter", i => i.QuarterValue);
            csvWriter.DefineColumn(@"Obligation type", i => i.ObligationType);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"PCS name", i => i.SchemeName);
            csvWriter.DefineColumn(@"PCS approval number", i => i.PcsApprovalNumber);
            csvWriter.DefineColumn(@"PCS appropriate authority", i => i.PcsAbbreviation);
            csvWriter.DefineColumn(@"AATF name", i => i.AatfName);
            csvWriter.DefineColumn(@"AATF approval number", i => i.AatfApprovalNumber);
            csvWriter.DefineColumn(@"AATF appropriate authority", i => i.AatfAbbreviation);
            csvWriter.DefineColumn(@"PCS report (t)", i => i.PcsTonnage);
            csvWriter.DefineColumn(@"AATF report(t)", i => i.AatfTonnage);
            csvWriter.DefineColumn(@"Discrepancy between PCS and AATF reports (t)", i => i.DifferenceTonnage);

            var fileContent = csvWriter.Write(items);

            var fileNameParameters = string.Empty;

            if (request.Quarter != null)
            {
                fileNameParameters += $"_Q{request.Quarter}";
            }

            if (!string.IsNullOrWhiteSpace(request.ObligationType))
            {
                fileNameParameters += $"_{request.ObligationType}";
            }

            var fileName = $"{request.ComplianceYear}{fileNameParameters}_PCS v AATF WEEE data comparison_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
