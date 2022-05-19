namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using System;
    using System.Threading.Tasks;

    public class GetPcsObligationsCsvHandler : IRequestHandler<GetPcsObligationsCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess dataAccess;

        public GetPcsObligationsCsvHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            CsvWriterFactory csvWriterFactory,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
            this.dataAccess = dataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetPcsObligationsCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthorityWithSchemes(message.Authority);

            var csvWriter = csvWriterFactory.Create<Scheme>();
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

            var fileContent = csvWriter.Write(authority.Schemes);
            var timestamp = DateTime.Now;
            var fileName = $"{authority.Abbreviation}_pcsobligationuploadtemplate{timestamp.ToString("dd/MM/yyyy_HHmm")}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
