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

            var items = await context.StoredProcedures.SpgProducerEEECSVDataByComplianceYearAndObligationType(
                request.ComplianceYear, obligationType);

            CsvWriter<ProducerEEECSVData> csvWriter =
                csvWriterFactory.Create<ProducerEEECSVData>();

            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer country", i => i.ProducerCountry);
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Total EEE", i => i.TotalTonnage);
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

            var fileName = string.Format("{0}_producerEEEData_{1:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        private static void WriteDataForCategoryAndQuarter(CsvWriter<ProducerEEECSVData> csvWriter, string obligation)
        {
            foreach (int category in Enumerable.Range(1, 14))
            {
                foreach (int quarterType in Enumerable.Range(1, 4))
                {
                    string title = string.Format("Cat{0} {1} (Q{2})", category, obligation, quarterType);
                    csvWriter.DefineColumn(title, i => string.Format("Cat{0}Q{1}", category, quarterType));
                }
            }
                    //csvWriter.DefineColumn(@"Cat1 (Q2)", i => i.Cat1Q2);
                    //csvWriter.DefineColumn(@"Cat1 (Q3)", i => i.Cat1Q3);
                    //csvWriter.DefineColumn(@"Cat1 (Q4)", i => i.Cat1Q4);

                    //csvWriter.DefineColumn(@"Cat2 (Q1)", i => i.Cat2Q1);
                    //csvWriter.DefineColumn(@"Cat2 (Q2)", i => i.Cat2Q2);
                    //csvWriter.DefineColumn(@"Cat2 (Q3)", i => i.Cat2Q3);
                    //csvWriter.DefineColumn(@"Cat2 (Q4)", i => i.Cat2Q4);

                    //csvWriter.DefineColumn(@"Cat3 (Q1)", i => i.Cat3Q1);
                    //csvWriter.DefineColumn(@"Cat3 (Q2)", i => i.Cat3Q2);
                    //csvWriter.DefineColumn(@"Cat3 (Q3)", i => i.Cat3Q3);
                    //csvWriter.DefineColumn(@"Cat3 (Q4)", i => i.Cat3Q4);

                    //csvWriter.DefineColumn(@"Cat4 (Q1)", i => i.Cat4Q1);
                    //csvWriter.DefineColumn(@"Cat4 (Q2)", i => i.Cat4Q2);
                    //csvWriter.DefineColumn(@"Cat4 (Q3)", i => i.Cat4Q3);
                    //csvWriter.DefineColumn(@"Cat4 (Q4)", i => i.Cat4Q4);

                    //csvWriter.DefineColumn(@"Cat5 (Q1)", i => i.Cat5Q1);
                    //csvWriter.DefineColumn(@"Cat5 (Q2)", i => i.Cat5Q2);
                    //csvWriter.DefineColumn(@"Cat5 (Q3)", i => i.Cat5Q3);
                    //csvWriter.DefineColumn(@"Cat5 (Q4)", i => i.Cat5Q4);

                    //csvWriter.DefineColumn(@"Cat6 (Q1)", i => i.Cat6Q1);
                    //csvWriter.DefineColumn(@"Cat6 (Q2)", i => i.Cat6Q2);
                    //csvWriter.DefineColumn(@"Cat6 (Q3)", i => i.Cat6Q3);
                    //csvWriter.DefineColumn(@"Cat6 (Q4)", i => i.Cat6Q4);                
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
