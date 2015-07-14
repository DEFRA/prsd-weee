namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class UpdateOrganisationTypeDetailsHandler : IRequestHandler<UpdateOrganisationTypeDetails, Guid>
    {
        private readonly WeeeContext db;

        public UpdateOrganisationTypeDetailsHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<Guid> HandleAsync(UpdateOrganisationTypeDetails message)
        {
            if (await db.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            organisation.UpdateOrganisationTypeDetails(message.Name, message.CompaniesRegistrationNumber, message.TradingName, GetAddressType(message.OrganisationType));
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return organisation.Id;
        }

        public Domain.OrganisationType GetAddressType(Core.Organisations.OrganisationType orgType)
        {
            switch (orgType)
            {
                case Core.Organisations.OrganisationType.RegisteredCompany:
                    return Domain.OrganisationType.RegisteredCompany;

                case Core.Organisations.OrganisationType.Partnership:
                    return Domain.OrganisationType.Partnership;

                case Core.Organisations.OrganisationType.SoleTraderOrIndividual:
                    return Domain.OrganisationType.SoleTraderOrIndividual;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", orgType),
                        "orgType");
            }
        }
    }
}
