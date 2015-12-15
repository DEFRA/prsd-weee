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

    internal class GetProducerPublicRegisterCSVHandler : IRequestHandler<GetProducerPublicRegisterCSV, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerPublicRegisterCSVHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducerPublicRegisterCSV request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (request.ComplianceYear == 0)
            {
                string message = string.Format("Compliance year cannot be \"{0}\".", request.ComplianceYear);
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(
                request.ComplianceYear);

            CsvWriter<ProducerPublicRegisterCSVData> csvWriter =
                csvWriterFactory.Create<ProducerPublicRegisterCSVData>();

            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer trading name", i => i.TradingName);
            csvWriter.DefineColumn(@"Obligation type", i => i.ObligationType);
            csvWriter.DefineColumn(@"Registered office address / principal place of business", i => !string.IsNullOrEmpty(i.CompanyName) ?
            ConcatAddress(new[]
            {
                i.ROAPrimaryName, i.ROASecondaryName, i.ROAStreet, i.ROATown, i.ROALocality, i.ROAAdministrativeArea, i.ROACountry, i.ROAPostCode
            })
            : ConcatAddress(new[]
            {
                i.PPOBPrimaryName, i.PPOBSecondaryName, i.PPOBStreet, i.PPOBTown, i.PPOBLocality, i.PPOBAdministrativeArea, i.PPOBCountry, i.PPOBPostcode
            }));
            csvWriter.DefineColumn(@"Registered office phone number", i => !string.IsNullOrEmpty(i.CompanyName) ? i.ROATelephone : string.Empty);
            csvWriter.DefineColumn(@"Registered office email address", i => !string.IsNullOrEmpty(i.CompanyName) ? i.ROAEmail : string.Empty);
            csvWriter.DefineColumn(@"Producer registration number (PRN)", i => i.PRN);
            csvWriter.DefineColumn(@"Producer compliance scheme (PCS) name", i => i.SchemeName);
            csvWriter.DefineColumn(@"PCS operator name", i => i.SchemeOperator);

            csvWriter.DefineColumn(@"PCS registered office", i => ConcatAddress(new[] { i.CSROAAddress1, i.CSROAAddress2, i.CSROATownOrCity, i.CSROACountyOrRegion, i.CSROACountry, i.CSROAPostcode }));
            csvWriter.DefineColumn(@"Overseas producer name and address", i => ConcatAddress(new[] { i.OPNAName, i.OPNAPrimaryName, i.OPNASecondaryName, i.OPNAStreet, i.OPNATown, i.OPNALocality, i.OPNAAdministrativeArea, i.OPNACountry, i.OPNAPostCode }));

            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear.ToString());

            string fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0}_producerpublicregister_{1:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        private string ConcatAddress(string[] addressElements)
        {
            return string.Join(", ", addressElements.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
