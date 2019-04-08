namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class AddAddressToOrganisationHandler : IRequestHandler<AddAddressToOrganisation, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public AddAddressToOrganisationHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(AddAddressToOrganisation message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var addresstype = ValueObjectInitializer.GetAddressType(message.TypeOfAddress);

            if (await db.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            if (await db.Countries.FirstOrDefaultAsync(c => c.Id == message.Address.CountryId) == null)
            {
                throw new ArgumentException(string.Format("Could not find country with id {0}",
                    message.Address.CountryId));
            }

            var country = await db.Countries.SingleAsync(c => c.Id == message.Address.CountryId);

            message.Address.CountryName = country.Name;
            var address = ValueObjectInitializer.CreateAddress(message.Address, country);

            if (addresstype.Equals(AddressType.SchemeAddress))
            {
                if (message.AddressId.HasValue)
                {
                    var findAddress = await db.Addresses.SingleAsync(a => a.Id == message.AddressId.Value);

                    findAddress.Overwrite(address);
                    address = findAddress;
                }
                else
                {
                    db.Addresses.Add(address);
                }
            }
            else
            {
                organisation.AddOrUpdateAddress(addresstype, address);
            }

            await db.SaveChangesAsync();
                
            return GetAddressId(addresstype, organisation, address);
        }

        private static Guid GetAddressId(AddressType type, Organisation organisation, Address address)
        {
            switch (type.Value)
            {
                case 1:
                    return address.Id;
                case 2:
                    return organisation.BusinessAddress.Id;
                case 3:
                    return organisation.NotificationAddress.Id;
                default:
                    throw new InvalidOperationException("No address id found.");
            }
        }
    }
}