namespace EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetProducerEeeDataCsvHandler : IRequestHandler<GetProducerEeeDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetProducerEeeDataCsvDataAccess dataAccess;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerEeeDataCsvHandler(
            IWeeeAuthorization authorization,
            IGetProducerEeeDataCsvDataAccess dataAccess,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducerEeeDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            string obligationType = ConvertEnumToDatabaseString(request.ObligationType);

            CsvWriter<ProducerEeeCsvData> csvWriter = CreateWriter(obligationType);

            List<ProducerEeeCsvData> items = await dataAccess.GetItemsAsync(
                request.ComplianceYear,
                request.SchemeId,
                obligationType);

            string fileContent = csvWriter.Write(items);

            string fileName;

            if (request.SchemeId == null)
            {
                fileName = string.Format("{0}_{1}_producerEEE_{2:ddMMyyyy_HHmm}.csv",
                    request.ComplianceYear,
                    obligationType,
                    DateTime.UtcNow);
            }
            else
            {
                Domain.Scheme.Scheme scheme = await dataAccess.GetSchemeAsync(request.SchemeId.Value);

                fileName = string.Format("{0}_{1}_{2}_producerEEE_{3:ddMMyyyy_HHmm}.csv",
                    request.ComplianceYear,
                    scheme.ApprovalNumber.Replace("/", string.Empty),
                    obligationType,
                    DateTime.UtcNow);
            }

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        public CsvWriter<ProducerEeeCsvData> CreateWriter(string obligationType)
        {
            CsvWriter<ProducerEeeCsvData> csvWriter = csvWriterFactory.Create<ProducerEeeCsvData>();

            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Scheme approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer country", i => i.ProducerCountry);
           
            string totalEEEtitle = string.Format("Total EEE {0} (t)", obligationType);
            csvWriter.DefineColumn(totalEEEtitle, i => i.TotalTonnage);
            
            foreach (int category in Enumerable.Range(1, 14))
            {
                foreach (int quarterType in Enumerable.Range(1, 4))
                {
                    string title = string.Format("Cat{0} {1} Q{2} (t)", category, obligationType, quarterType);
                    string columnName = string.Format("Cat{0}Q{1}", category, quarterType);
                    csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));
                }
            }

            return csvWriter;
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
