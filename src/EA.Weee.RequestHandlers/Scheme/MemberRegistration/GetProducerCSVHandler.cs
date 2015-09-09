namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Core.Scheme;
    using DataAccess;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class GetProducerCSVHandler : IRequestHandler<GetProducerCSV, ProducerCSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerCSVHandler(IWeeeAuthorization authorization, WeeeContext context, CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<ProducerCSVFileData> HandleAsync(GetProducerCSV request)
        {
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            var organisation = await context.Organisations.FindAsync(request.OrganisationId);

            if (organisation == null)
            {
                string message = string.Format("An organisation could not be found with ID \"{0}\".", request.OrganisationId);
                throw new ArgumentException(message);
            }

            var items = await context.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(
                request.OrganisationId,
                request.ComplianceYear);

            CsvWriter<ProducerCsvData> csvWriter = csvWriterFactory.Create<ProducerCsvData>();
            csvWriter.DefineColumn("Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn("Trading name", i => i.TradingName);
            csvWriter.DefineColumn("Producer registration number", i => i.RegistrationNumber);
            csvWriter.DefineColumn("Company registration number", i => i.CompanyNumber);
            csvWriter.DefineColumn("Charge band", i => i.ChargeBand);
            csvWriter.DefineColumn("Date & time (GMT) registered", i => i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss"));
            csvWriter.DefineColumn("Date & time (GMT) last updated", i => (i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss").Equals(i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")) ? string.Empty : i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")));
            csvWriter.DefineColumn("Authorised representative", i => i.AuthorisedRepresentative);
            csvWriter.DefineColumn("Overseas producer", i => i.OverseasProducer);

            string fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0:yyyy_MM_dd} - {1}.csv",
                DateTime.Now,
                request.ComplianceYear);
            
            return new ProducerCSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
