namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.AatfReports;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
    public class GetAllAatfReuseSitesCsvHandler : IRequestHandler<GetAllAatfReuseSitesCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAllAatfReuseSitesCsvHandler(IWeeeAuthorization authorization, WeeeContext weeContext,
                 CsvWriterFactory csvWriterFactory, ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.commonDataAccess = commonDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAllAatfReuseSitesCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var reuseSitesData = await weeContext.StoredProcedures.GetAllAatfReuseSitesCsvData(request.ComplianceYear, request.AuthorityId, request.PanArea);

            var csvWriter = csvWriterFactory.Create<AatfReuseSitesData>();

            csvWriter.DefineColumn(@"Appropriate authority", i => i.Abbreviation);
            csvWriter.DefineColumn(@"WROS Pan Area Team", i => i.PanName);
            csvWriter.DefineColumn(@"EA Area", i => i.LaName);
            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Date submitted (GMT)", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Name of AATF", i => i.Name);
            csvWriter.DefineColumn(@"Approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrgName);
            csvWriter.DefineColumn(@"Reuse site name", i => i.SiteName);
            csvWriter.DefineColumn(@"Reuse site address", i => i.SiteAddress);

            var fileContent = csvWriter.Write(reuseSitesData);

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
            fileName += $"_AATF using reuse sites_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
