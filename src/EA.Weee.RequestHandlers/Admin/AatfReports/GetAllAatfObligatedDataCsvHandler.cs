namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.RequestHandlers.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
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
            PanArea panArea = null;
            UKCompetentAuthority authority = null;
            if (request.AuthorityId != null)
            {
                authority = await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value);
            }
            if (request.PanArea != null)
            {
                panArea = await commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value);
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

           var fileName = string.Format("{0}", request.ComplianceYear);
            if (request.AuthorityId != null)
            {
                fileName += "_" + authority.Abbreviation;
            }
            if (request.PanArea != null)
            {
                fileName += "_" + panArea.Name;
            }
            if (!string.IsNullOrEmpty(request.AATFName))
            {
                fileName += "_" + request.AATFName;
            }
            if (!string.IsNullOrEmpty(request.ObligationType))
            {
                fileName += "_" + request.ObligationType;
            }

            fileName += string.Format("_{0:ddMMyyyy}_{0:HHmm}.csv",
                                SystemTime.UtcNow);           

            string fileContent = DataTableCsvHelper.DataTableToCSV(obligatedData);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }   
}
