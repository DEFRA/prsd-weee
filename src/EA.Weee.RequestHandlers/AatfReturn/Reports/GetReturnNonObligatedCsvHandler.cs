namespace EA.Weee.RequestHandlers.AatfReturn.Reports
{
    using Core.Admin;
    using DataAccess;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn.Reports;
    using Security;
    using Shared;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess.StoredProcedure;

    public class GetReturnNonObligatedCsvHandler : IRequestHandler<GetReturnNonObligatedCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetReturnNonObligatedCsvHandler(IWeeeAuthorization authorization,
            WeeeContext weeContext,
            IReturnDataAccess returnDataAccess, 
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.returnDataAccess = returnDataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetReturnNonObligatedCsv request)
        {
            var @return = await returnDataAccess.GetById(request.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Organisation.Id);

            var items = await weeContext.StoredProcedures.GetReturnNonObligatedCsvData(@return.Id);

            var fileName =
                $"{@return.Quarter.Year}_{@return.Quarter.Q}_{@return.Organisation.OrganisationName}_Non-obligated return data_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            var csvWriter = csvWriterFactory.Create<NonObligatedWeeeReceivedCsvData>();
            csvWriter.DefineColumn(@"Compliance year", i => i.Year);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Submitted date (GMT)", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Name of operator", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Category", i => i.Category);
            csvWriter.DefineColumn(@"Total non-obligated WEEE received (t)", i => i.TotalNonObligatedWeeeReceived);
            csvWriter.DefineColumn(@"Non-obligated WEEE kept / retained by DCFs (t)", i => i.TotalNonObligatedWeeeReceivedFromDcf);
            var fileContent = csvWriter.Write(items);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
