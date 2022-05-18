namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Obligations;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetPcsObligationsCsvHandler : IRequestHandler<GetPcsObligationsCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetPcsObligationsCsvHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetPcsObligationsCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            var schemeAuthorityData = await context.UKCompetentAuthorities.Include(x => x.Schemes).Where(x => x.Id == message.AuthorityId).FirstOrDefaultAsync();

            var csvWriter = csvWriterFactory.Create<Scheme>();
            csvWriter.DefineColumn(ObligationCsvConstants.SchemeIdentifierColumnName, x => x.ApprovalNumber);
            csvWriter.DefineColumn(ObligationCsvConstants.SchemeNameColumnName, x => x.SchemeName);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat1ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat2ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat3ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat4ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat5ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat6ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat7ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat8ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat9ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat10ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat11ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat12ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat13ColumnName, null);
            csvWriter.DefineColumn(ObligationCsvConstants.Cat14ColumnName, null);

            var fileContent = csvWriter.Write(schemeAuthorityData.Schemes);
            var timestamp = DateTime.Now;
            var fileName = $"{schemeAuthorityData.Abbreviation}_pcsobligationuploadtemplate{timestamp.ToString("dd/MM/yyyy_HHmm")}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
