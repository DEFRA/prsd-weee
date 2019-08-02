namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using Prsd.Core;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using Shared;

    internal class GetAatfAeReturnDataCsvHandler : IRequestHandler<GetAatfAeReturnDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAatfAeReturnDataCsvHandler(IWeeeAuthorization authorization, 
            WeeeContext context,
            CsvWriterFactory csvWriterFactory, 
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetAatfAeReturnDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetAatfAeReturnDataCsvData(
                       request.ComplianceYear, request.Quarter, (int)request.FacilityType, 
                       request.ReturnStatus.HasValue ? (int)request.ReturnStatus : (int?)null, request.AuthorityId, request.LocalArea, request.PanArea, request.IncludeReSubmissions);

            foreach (var item in items)
            {
                 item.AatfDataUrl = $@" =HYPERLINK(""""{request.AatfDataUrl}{item.AatfId}#data"""", """"View AATF / AE data"""")";
            }

            var csvWriter = csvWriterFactory.Create<AatfAeReturnData>();
            csvWriter.DefineColumn(@"Name of AATF / AE", i => i.Name);
            csvWriter.DefineColumn(@"Approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Submission status", i => i.ReturnStatus);
            csvWriter.DefineColumn(@"Date created (GMT)", i => i.CreatedDate);
            csvWriter.DefineColumn(@"Date submitted (GMT)", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Appropriate authority", i => i.CompetentAuthorityAbbr);
            csvWriter.DefineColumn(@"First submission / resubmission", i => i.ReSubmission);
            csvWriter.DefineColumn(@" ", i => i.AatfDataUrl);
            var fileContent = csvWriter.Write(items);

            //Trim the space before equals in  =Hyperlink
            fileContent = fileContent.Replace(" =HYPERLINK", "=HYPERLINK");

            var excludeResubmissions = request.IncludeReSubmissions ? "Include resubmissions" : "Exclude resubmissions";

            var additionalParameters = string.Empty;
            if (request.ReturnStatus.HasValue)
            {
                additionalParameters = $"_{EnumHelper.GetDisplayName(request.ReturnStatus.Value)}";
            }
            if (request.AuthorityId.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Abbreviation}";
            }
            if (request.PanArea.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Name}";
            }
            if (request.LocalArea.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Name}";
            }

            var fileName =
                $"{request.ComplianceYear}_Q{request.Quarter}_{excludeResubmissions}_{request.FacilityType.ToString().ToUpper()}{additionalParameters}_Summary of AATF-AE returns to date_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
