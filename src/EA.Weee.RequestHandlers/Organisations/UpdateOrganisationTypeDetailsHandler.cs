namespace EA.Weee.RequestHandlers.Organisations
{
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Security;

    internal class UpdateOrganisationTypeDetailsHandler : IRequestHandler<UpdateOrganisationTypeDetails, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public UpdateOrganisationTypeDetailsHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateOrganisationTypeDetails message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisation = await db.Organisations.SingleOrDefaultAsync(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            organisation.UpdateOrganisationTypeDetails(message.Name, message.CompaniesRegistrationNumber, message.TradingName, GetOrganisationType(message.OrganisationType));
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

        public OrganisationType GetOrganisationType(Core.Organisations.OrganisationType orgType)
        {
            switch (orgType)
            {
                case Core.Organisations.OrganisationType.RegisteredCompany:
                    return OrganisationType.RegisteredCompany;

                case Core.Organisations.OrganisationType.Partnership:
                    return OrganisationType.Partnership;

                case Core.Organisations.OrganisationType.SoleTraderOrIndividual:
                    return OrganisationType.SoleTraderOrIndividual;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", orgType),
                        "orgType");
            }
        }
    }
}
