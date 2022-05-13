namespace EA.Weee.RequestHandlers.Admin
{
    using Domain.Organisation;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class CreateOrganisationAdminHandler : IRequestHandler<CreateOrganisationAdmin, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly WeeeContext db;

        public CreateOrganisationAdminHandler(IWeeeAuthorization authorization, IGenericDataAccess dataAccess, WeeeContext context)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.db = context;
        }

        public async Task<Guid> HandleAsync(CreateOrganisationAdmin message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            Organisation organisation = null;

            switch (message.OrganisationType)
            {
                case Core.Organisations.OrganisationType.Partnership:
                    organisation = Organisation.CreatePartnership(message.BusinessName);
                    break;
                case Core.Organisations.OrganisationType.RegisteredCompany:
                    organisation = Organisation.CreateRegisteredCompany(message.BusinessName, message.RegistrationNumber, message.TradingName);
                    break;
                case Core.Organisations.OrganisationType.SoleTraderOrIndividual:
                    organisation = Organisation.CreateSoleTrader(message.BusinessName, message.TradingName);
                    break;
                default:
                    break;
            }

            if (organisation == null)
            {
                throw new NotImplementedException("This organisation type hasn't been implented");
            }

            Country country = await db.Countries.SingleAsync(c => c.Id == message.Address.CountryId);

            Address address = ValueObjectInitializer.CreateAddress(message.Address, country);

            await dataAccess.Add<Address>(address);

            organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);

            var result = await dataAccess.Add<Organisation>(organisation);

            return result.Id;
        }
    }
}
