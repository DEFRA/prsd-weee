namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.RequestHandlers.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using System;
    using System.Threading.Tasks;

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

            var obligatedData = await weeContext.StoredProcedures.GetAllAatfObligatedCsvData(request.ComplianceYear, request.AATFName, request.ObligationType, request.AuthorityId, request.PanArea, request.ColumnType);

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

            var fileName = $"{request.ComplianceYear}";
            if (request.AuthorityId.HasValue)
            {
                var authority = await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value);
                fileName += "_" + authority.Abbreviation;
            }
            if (request.PanArea.HasValue)
            {
                var panArea = await commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value);
                fileName += "_" + panArea.Name;
            }
            if (!string.IsNullOrEmpty(request.ObligationType))
            {
                fileName += "_" + request.ObligationType;
            }

            fileName += $"_AATF obligated WEEE data_{SystemTime.UtcNow:ddMMyyyy}_{SystemTime.UtcNow:HHmm}.csv";

            var fileContent = obligatedData.DataTableToCsv();

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
