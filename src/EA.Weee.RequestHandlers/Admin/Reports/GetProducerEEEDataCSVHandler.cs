namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetProducerEEEDataCSVHandler : IRequestHandler<GetProducersEEEDataCSV, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerEEEDataCSVHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducersEEEDataCSV request)
        {
            authorization.EnsureCanAccessInternalArea();

            string obligationType = ConvertEnumToDatabaseString(request.ObligationType);

            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            string fileContent = await GetProducersEEEDataCSVContent(request.ComplianceYear, obligationType);

            var fileName = string.Format("{0}_{1}_producerEEEData_{2:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                obligationType,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        private async Task<string> GetProducersEEEDataCSVContent(int complianceYear, string obligationType)
        {
            var items = await context.StoredProcedures.SpgProducerEEECSVDataByComplianceYearAndObligationType(
                complianceYear, obligationType);

            CsvWriter<ProducerEEECSVData> csvWriter =
                csvWriterFactory.Create<ProducerEEECSVData>();

            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer country", i => i.ProducerCountry);
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            string totalEEEtitle = string.Format("Total EEE ({0})", obligationType);
            csvWriter.DefineColumn(totalEEEtitle, i => i.TotalTonnage);
            
            foreach (int category in Enumerable.Range(1, 14))
            {
                foreach (int quarterType in Enumerable.Range(1, 4))
                {
                    string title = string.Format("Cat{0} {1} (Q{2})", category, obligationType, quarterType);
                    string columnName = string.Format("Cat{0}Q{1}", category, quarterType);
                    csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));
                }
            }

            string fileContent = csvWriter.Write(items);
            return fileContent;
        }
        private static string ConvertEnumToDatabaseString(ObligationType obligationType)
        {
            switch (obligationType)
            {
                case ObligationType.B2B:
                    return "B2B";

                case ObligationType.B2C:
                    return "B2C";
                
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
