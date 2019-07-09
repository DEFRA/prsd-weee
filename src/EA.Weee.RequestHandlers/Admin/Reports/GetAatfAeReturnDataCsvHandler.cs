namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core.Helpers;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetAatfAeReturnDataCsvHandler : IRequestHandler<GetAatfAeReturnDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetAatfAeReturnDataCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
          CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }
        public async Task<CSVFileData> HandleAsync(GetAatfAeReturnDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.GetAatfAeReturnDataCsvData(
                       request.ComplianceYear, request.Quarter, (int)request.FacilityType, 
                       request.ReturnStatus, request.AuthorityId, request.LocalArea, request.PanArea);

            CsvWriter<AatfAeReturnData> csvWriter = csvWriterFactory.Create<AatfAeReturnData>();
            csvWriter.DefineColumn(@"Name of AATF / AE", i => i.Name);
            csvWriter.DefineColumn(@"Approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Submission status", i => i.ReturnStatus);
            csvWriter.DefineColumn(@"Date created", i => i.CreatedDate);
            csvWriter.DefineColumn(@"Date submitted", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Submitted by", i => i.SubmittedBy);
            csvWriter.DefineColumn(@"Appropriate authority", i => i.CompetentAuthorityAbbr);
            var fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0}_AATF_AE_RETURN_{1:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
