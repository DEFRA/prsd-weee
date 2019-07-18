namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.RequestHandlers.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetAllAatfObligatedDataCsvHandler : IRequestHandler<GetAllAatfObligatedDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetAllAatfObligatedDataCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
          CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAllAatfObligatedDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

           var dlist = await context.StoredProcedures.GetAllAatfObligatedCsvData(request.ComplianceYear, request.AATFName, request.ObligationType, request.AuthorityId, request.PanArea, request.ColumnType);

            string fileContent = DataTableCsvHelper.DataTableToCSV(dlist);

            var fileName = string.Format("{0}_AA_Obligated_{1:ddMMyyyy_HHmm}.csv",
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
