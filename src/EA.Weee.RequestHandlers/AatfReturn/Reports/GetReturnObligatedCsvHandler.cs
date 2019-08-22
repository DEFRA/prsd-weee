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

    public class GetReturnObligatedCsvHandler : IRequestHandler<GetReturnObligatedCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly IReturnDataAccess returnDataAccess;

        public GetReturnObligatedCsvHandler(IWeeeAuthorization authorization,
            WeeeContext weeContext,
            IReturnDataAccess returnDataAccess)
        {
            this.authorization = authorization;
            this.weeContext = weeContext;
            this.returnDataAccess = returnDataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetReturnObligatedCsv request)
        {
            var @return = await returnDataAccess.GetById(request.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Organisation.Id);

            var obligatedData = await weeContext.StoredProcedures.GetReturnObligatedCsvData(@return.Id);

            obligatedData.SetColumnsOrder("Compliance Year", "Quarter", "Name of AATF", "AATF approval number", "Submitted by", "Submitted date (GMT)", "Category", "Obligation");

            var fileName =
                $"{@return.Quarter.Year}_{@return.Quarter.Q}_{@return.Organisation.OrganisationName}_Obligated return data_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            var fileContent = obligatedData.DataTableToCsv(new List<string>(new string[] { "ReturnId", "AatfKey", "ObligationType", "CategoryId", "AatfId" }));

            obligatedData.Dispose();

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
