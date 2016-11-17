namespace EA.Weee.RequestHandlers.Admin.Reports.GetMissingProducerDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    internal class GetMissingProducerDataCsvHandler : IRequestHandler<GetMissingProducerDataCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetMissingProducerDataCsvDataProcessor dataProcessor;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetMissingProducerDataCsvHandler(
            IWeeeAuthorization authorization,
            IGetMissingProducerDataCsvDataProcessor dataProcessor,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataProcessor = dataProcessor;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetMissingProducerDataCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            string obligationType = ConvertEnumToDatabaseString(request.ObligationType);

            CsvWriter<MissingProducerDataCsvData> csvWriter = CreateWriter();

            List<MissingProducerDataCsvData> items = await dataProcessor.FetchMissingProducerDataAsync(request.ComplianceYear,
                obligationType, request.Quarter, request.SchemeId);

            string fileContent = csvWriter.Write(items);

            string fileName = string.Format("{0}_producermissing{1}data_{2:ddMMyyyy_HHmm}.csv",
                    request.ComplianceYear,
                    obligationType,
                    DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        public CsvWriter<MissingProducerDataCsvData> CreateWriter()
        {
            CsvWriter<MissingProducerDataCsvData> csvWriter = csvWriterFactory.Create<MissingProducerDataCsvData>();

            csvWriter.DefineColumn(@"PCS name", i => i.SchemeName);
            csvWriter.DefineColumn(@"PCS approval no.", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer PRN", i => i.PRN);
            csvWriter.DefineColumn(@"Obligation type", i => i.ObligationType);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Date registered", i => i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss"));

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

                case ObligationType.Both:
                case ObligationType.None:
                default:
                    throw new NotSupportedException();
            }
        }
    }
}