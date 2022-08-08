namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using System.Linq;
    using System.Threading.Tasks;
    using Prsd.Core;
    using Weee.Security;

    public class GetPcsObligationsCsvHandler : IRequestHandler<GetPcsObligationsCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICsvWriter<Scheme> csvWriter;
        private readonly ICommonDataAccess dataAccess;

        public GetPcsObligationsCsvHandler(IWeeeAuthorization authorization,
            ICsvWriter<Scheme> csvWriter,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.csvWriter = csvWriter;
            this.dataAccess = dataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetPcsObligationsCsv message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthorityApprovedSchemes(message.Authority);

            csvWriter.DefineColumn(ObligationCsvConstants.SchemeIdentifierColumnName, x => x.ApprovalNumber);
            csvWriter.DefineColumn(ObligationCsvConstants.SchemeNameColumnName, x => x.SchemeName);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat1ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat2ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat3ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat4ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat5ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat6ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat7ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat8ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat9ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat10ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat11ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat12ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat13ColumnName, x => string.Empty);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat14ColumnName, x => string.Empty);

            var fileContent = csvWriter.Write(authority.Schemes.OrderBy(s => s.SchemeName));
            var timestamp = SystemTime.Now;
            var fileName = $"{authority.Abbreviation}_pcsobligationuploadtemplate{timestamp.ToString(DateTimeConstants.FilenameTimestampFormat)}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
