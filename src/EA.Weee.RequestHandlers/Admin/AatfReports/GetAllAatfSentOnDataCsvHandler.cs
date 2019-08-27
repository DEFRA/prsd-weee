namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using System;
    using System.Threading.Tasks;
    using Prsd.Core;

    public class GetAllAatfSentOnDataCsvHandler : IRequestHandler<GetAllAatfSentOnDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAllAatfSentOnDataCsvHandler(IWeeeAuthorization authorization, WeeeContext weeContext,
          CsvWriterFactory csvWriterFactory, ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.commonDataAccess = commonDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAllAatfSentOnDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var obligatedData = await weeContext.StoredProcedures.GetAllAatfSentOnDataCsv(request.ComplianceYear, request.ObligationType, request.AuthorityId, request.PanArea);

            //Remove the Id columns
            if (obligatedData.Tables.Count > 0 && obligatedData.Tables[0] != null)
            {
                if (obligatedData.Tables[0].Columns.Contains("AatfId"))
                {
                    obligatedData.Tables[0].Columns.Remove("AatfId");
                }
                if (obligatedData.Tables[0].Columns.Contains("ReturnId"))
                {
                    obligatedData.Tables[0].Columns.Remove("ReturnId");
                }
                if (obligatedData.Tables[0].Columns.Contains("Q"))
                {
                    obligatedData.Tables[0].Columns.Remove("Q");
                }
                if (obligatedData.Tables[0].Columns.Contains("CategoryId"))
                {
                    obligatedData.Tables[0].Columns.Remove("CategoryId");
                }
                if (obligatedData.Tables[0].Columns.Contains("TonnageType"))
                {
                    obligatedData.Tables[0].Columns.Remove("TonnageType");
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

            fileName += $"_AATF WEEE sent on for treatment_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            var fileContent = string.Empty;
            if (obligatedData.Tables.Count > 0)
            {
                fileContent = obligatedData.Tables[0].DataSetSentOnToCsv(obligatedData.Tables[1]);
            }

            obligatedData.Dispose();

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
