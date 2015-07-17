namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;

    internal class GetProducerCSVByComplianceYearHandler : IRequestHandler<GetProducerCSVByComplianceYear, string>
    {
        private readonly WeeeContext context;

        public GetProducerCSVByComplianceYearHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<string> HandleAsync(GetProducerCSVByComplianceYear message)
        {
            var organisation = await context.Organisations.SingleOrDefaultAsync(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var scheme = await context.Schemes.SingleAsync(s => s.OrganisationId == message.OrganisationId);

            var csvData = scheme.GetProducerCSV(message.ComplianceYear);

            return csvData;
        }
    }
}
