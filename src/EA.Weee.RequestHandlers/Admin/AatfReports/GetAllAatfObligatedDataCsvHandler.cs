namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    public class GetAllAatfObligatedDataCsvHandler : IRequestHandler<GetAllAatfObligatedDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAllAatfObligatedDataCsvHandler(IWeeeAuthorization authorization, WeeeContext weeContext,
          CsvWriterFactory csvWriterFactory, ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.commonDataAccess = commonDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAllAatfObligatedDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }
            PanArea panArea = new PanArea();
            if (request.PanArea != null)
            {
                panArea = await commonDataAccess.FetchLookup<PanArea>((Guid)request.PanArea);
            }

            var dlist = await weeContext.StoredProcedures.GetAllAatfObligatedCsvData(request.ComplianceYear, request.AATFName, request.ObligationType, request.AuthorityId, request.PanArea, request.ColumnType);
 
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

           var fileName = string.Format("{0}_AA_{1:ddMMyyyy_HHmm}",
                request.ComplianceYear,
                DateTime.UtcNow);
            if (request.PanArea != null)
            {
                fileName += "_" + panArea.Name;
            }
            if (request.AATFName != null)
            {
                fileName += "_" + request.AATFName;
            }
            if (request.ObligationType != null)
            {
                fileName += "_" + request.ObligationType;
            }

            fileName += ".csv";

            string fileContent = DataTableCsvHelper.DataTableToCSV(dlist);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }   
}
