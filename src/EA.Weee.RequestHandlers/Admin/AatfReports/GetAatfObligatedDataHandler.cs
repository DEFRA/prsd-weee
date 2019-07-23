namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.Aatf;
    using Prsd.Core.Mediator;
    using Security;

    public class GetAatfObligatedDataHandler : IRequestHandler<GetAatfObligatedData, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IGetAatfsDataAccess aatfDataAccess;

        public GetAatfObligatedDataHandler(IWeeeAuthorization authorization, WeeeContext weeContext,
          CsvWriterFactory csvWriterFactory, IGetAatfsDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.aatfDataAccess = aatfDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAatfObligatedData request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var aatf = await aatfDataAccess.GetAatfById(request.AatfId);

            var dlist = await weeContext.StoredProcedures.GetAatfObligatedCsvData(request.ReturnId, request.ComplianceYear, request.Quarter, request.AatfId);

            //Remove the Id columns
            if (dlist != null)
            {
                if (dlist.Columns.Contains("AatfId"))
                {
                    dlist.Columns.Remove("AatfId");
                }
                if (dlist.Columns.Contains("ReturnId"))
                {
                    dlist.Columns.Remove("ReturnId");
                }
                if (dlist.Columns.Contains("Q"))
                {
                    dlist.Columns.Remove("Q");
                }
                if (dlist.Columns.Contains("CategoryId"))
                {
                    dlist.Columns.Remove("CategoryId");
                }
                if (dlist.Columns.Contains("TonnageType"))
                {
                    dlist.Columns.Remove("TonnageType");
                }
            }

            var fileName = string.Format("{0}-{1}{2}-{3:ddMMyyyy_HHmm}.csv",
                 aatf.Name, request.ComplianceYear, (QuarterType)request.Quarter,
                 DateTime.UtcNow);

            string fileContent = DataTableCsvHelper.DataTableToCSV(dlist);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
