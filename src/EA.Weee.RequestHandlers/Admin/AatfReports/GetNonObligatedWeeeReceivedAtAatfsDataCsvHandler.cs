namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.Requests.Admin.AatfReports;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using Shared;

    internal class GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler : IRequestHandler<GetUkNonObligatedWeeeReceivedAtAatfsDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory, 
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetUkNonObligatedWeeeReceivedAtAatfsDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetNonObligatedWeeeReceivedAtAatfs(request.ComplianceYear, request.AuthorityId, request.PatAreaId, request.AatfName);

            var csvWriter = csvWriterFactory.Create<NonObligatedWeeeReceivedAtAatfsData>();
            csvWriter.DefineColumn(@"Authority", i => i.Authority);
            csvWriter.DefineColumn(@"Pat Area", i => i.PatArea);
            csvWriter.DefineColumn(@"Area", i => i.Area);
            csvWriter.DefineColumn(@"Year", i => i.Year);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Date submitted", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total non-obligated WEEE received (t)", i => i.TotalNonObligatedWeeeReceived);
            csvWriter.DefineColumn(@"Non-obligated WEEE kept / retained by DCFs (t)", i => i.TotalNonObligatedWeeeReceivedFromDcf);
            var fileContent = csvWriter.Write(items);

            var fileNameParameters = string.Empty;
            if (request.AuthorityId.HasValue)
            {
                var authority = await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value);
                fileNameParameters += $"_{authority.Abbreviation}";
            }

            if (request.PatAreaId.HasValue)
            {
                var patArea = await commonDataAccess.FetchLookup<PanArea>(request.PatAreaId.Value);
                fileNameParameters += $"_{patArea.Name}";
            }

            if (!string.IsNullOrWhiteSpace(request.AatfName))
            {
                fileNameParameters += $"_{request.AatfName}";
            }

            var fileName = $"{request.ComplianceYear}{fileNameParameters}_AATF non-obligated WEEE data_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
