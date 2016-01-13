namespace EA.Weee.RequestHandlers.Charges.FetchIssuedChargesCsv
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using Security;

    public class FetchIssuedChargesCsvHandler : IRequestHandler<Requests.Charges.FetchIssuedChargesCsv, FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchIssuedChargesCsvDataAccess dataAccess;
        private readonly CsvWriterFactory csvWriterFactory;

        public FetchIssuedChargesCsvHandler(
            IWeeeAuthorization authorization,
            IFetchIssuedChargesCsvDataAccess dataAccess,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<FileInfo> HandleAsync(Requests.Charges.FetchIssuedChargesCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IEnumerable<ProducerSubmission> results = await dataAccess.FetchInvoicedProducerSubmissionsAsync(authority, message.ComplianceYear, message.SchemeName);

            CsvWriter<ProducerSubmission> csvWriter = csvWriterFactory.Create<ProducerSubmission>();

            csvWriter.DefineColumn("Scheme name", ps => ps.RegisteredProducer.Scheme.SchemeName);
            csvWriter.DefineColumn("Compliance year", ps => ps.RegisteredProducer.ComplianceYear);
            csvWriter.DefineColumn("Submission date and time (GMT)", ps => ps.MemberUpload.SubmittedDate.Value.ToString("dd/MM/yyyy HH:mm:ss"));
            csvWriter.DefineColumn("Producer name", ps => ps.OrganisationName);
            csvWriter.DefineColumn("PRN", ps => ps.RegisteredProducer.ProducerRegistrationNumber);
            csvWriter.DefineColumn("Charge value", ps => ps.ChargeThisUpdate);
            csvWriter.DefineColumn("Charge band", ps => ps.ChargeBandAmount.ChargeBand);
            csvWriter.DefineColumn("Issued date", ps => ps.MemberUpload.InvoiceRun.IssuedDate.ToString("dd/MM/yyyy HH:mm:ss"));

            string content = csvWriter.Write(results);
            byte[] data = Encoding.UTF8.GetBytes(content);

            // TODO: Do we need to add the scheme name or the current date to the file name?
            string fileName = string.Format(
                "issuedcharges_{0}_{1}.csv",
                authority.Abbreviation,
                message.ComplianceYear);

            return new FileInfo(fileName, data);
        }
    }
}
