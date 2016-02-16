namespace EA.Weee.RequestHandlers.Charges.FetchIssuedChargesCsv
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Requests.Charges;
    using Security;

    public class FetchIssuedChargesCsvHandler : IRequestHandler<FetchIssuedChargesCsv, FileInfo>
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

        public async Task<FileInfo> HandleAsync(FetchIssuedChargesCsv message)
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
            csvWriter.DefineColumn("Charge value (£)", ps => ps.ChargeThisUpdate);
            csvWriter.DefineColumn("Charge band", ps => ps.ChargeBandAmount.ChargeBand);
            csvWriter.DefineColumn("Issued date", ps => ps.MemberUpload.InvoiceRun.IssuedDate.ToString("dd/MM/yyyy HH:mm:ss"));
            
            string content = csvWriter.Write(results);
            byte[] data = Encoding.UTF8.GetBytes(content);

            string schemeApprovalNumber = string.Empty;            
            string fileName = string.Empty;

            if (!string.IsNullOrEmpty(message.SchemeName))
            {
                //get approval number for scheme to display in the filename.
                Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeAsync(message.SchemeName);
                schemeApprovalNumber = scheme.ApprovalNumber.Replace("/", string.Empty);
                fileName = string.Format(
                    "{0}_{1}_issuedcharges_{2:ddMMyyyy_HHmm}.csv",
                    message.ComplianceYear,                    
                    schemeApprovalNumber,                   
                    SystemTime.UtcNow);
            }
            else
            {
                fileName = string.Format(
                    "{0}_issuedcharges_{1:ddMMyyyy_HHmm}.csv",
                    message.ComplianceYear,
                    SystemTime.UtcNow);
            }
            return new FileInfo(fileName, data);
        }
    }
}
