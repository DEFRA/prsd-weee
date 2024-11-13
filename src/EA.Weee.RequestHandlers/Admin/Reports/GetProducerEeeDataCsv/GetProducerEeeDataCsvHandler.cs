namespace EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core;
    using EA.Weee.Core.Constants;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

            var obligationType = ConvertEnumToDatabaseString(request.ObligationType);

            var csvWriter = CreateWriter(obligationType);

            var items = await dataAccess.GetItemsAsync(
                request.ComplianceYear,
                request.SchemeId,
                obligationType);

            var fileContent = csvWriter.Write(items);

            string fileName;

            if (request.SchemeId == null)
            {
                fileName = $"{request.ComplianceYear}_{obligationType}_producerEEE_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";
            }
            else if (request.SchemeId == DirectRegistrantFixedIdConstant.DirectRegistrantFixedId)
            {
                fileName = $"{request.ComplianceYear}_{DirectRegistrantFixedIdConstant.DirectRegistrant}_{obligationType}_producerEEE_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";
            }
            else if (request.SchemeId == DirectRegistrantFixedIdConstant.SchemeFixedId)
            {
                fileName = $"{request.ComplianceYear}_{DirectRegistrantFixedIdConstant.AllSchemes}_{obligationType}_producerEEE_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";
            }
            else
            {
                Domain.Scheme.Scheme scheme = await dataAccess.GetSchemeAsync(request.SchemeId.Value);

                fileName = $"{request.ComplianceYear}_{scheme.ApprovalNumber.Replace("/", string.Empty)}_{obligationType}_producerEEE_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";
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

            csvWriter.DefineColumn(@"Scheme name or direct registrant", i => i.SchemeName);
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
