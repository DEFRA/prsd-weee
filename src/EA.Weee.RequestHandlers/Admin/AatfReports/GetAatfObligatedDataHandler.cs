namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Prsd.Core;
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

            var obligatedData = await weeContext.StoredProcedures.GetAatfObligatedCsvData(request.ReturnId, request.ComplianceYear, request.Quarter, request.AatfId);

            //Remove the Id columns
            if (obligatedData != null)
            {
                if (obligatedData.Columns.Contains("AatfId"))
                {
                    obligatedData.Columns.Remove("AatfId");
                }
                if (obligatedData.Columns.Contains("ReturnId"))
                {
                    obligatedData.Columns.Remove("ReturnId");
                }
                if (obligatedData.Columns.Contains("Q"))
                {
                    obligatedData.Columns.Remove("Q");
                }
                if (obligatedData.Columns.Contains("CategoryId"))
                {
                    obligatedData.Columns.Remove("CategoryId");
                }
                if (obligatedData.Columns.Contains("TonnageType"))
                {
                    obligatedData.Columns.Remove("TonnageType");
                }
            }

            var fileName = string.Format("{1}_{2}_{4}_Full data_{3:ddMMyyyy}_{3:HHmm}.csv",
                 aatf.Name, 
                 request.ComplianceYear, 
                 (QuarterType)request.Quarter,
                 SystemTime.UtcNow,
                 aatf.ApprovalNumber);

            string fileContent = DataTableCsvHelper.DataTableToCsv(obligatedData);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
