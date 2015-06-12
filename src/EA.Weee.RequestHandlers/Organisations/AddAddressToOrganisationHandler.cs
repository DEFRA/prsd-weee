namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class AddAddressToOrganisationHandler : IRequestHandler<AddAddressToOrganisation, Guid>
    {
        private readonly WeeeContext db;

        public AddAddressToOrganisationHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<Guid> HandleAsync(AddAddressToOrganisation message)
        {
            var addresstype = ValueObjectInitializer.GetAddressType(message.TypeOfAddress);
            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            var address = ValueObjectInitializer.CreateAddress(message.Address);

            organisation.AddAddress(addresstype, address);
            await db.SaveChangesAsync();

            return GetAddressId(addresstype, organisation);
        }

        private static Guid GetAddressId(AddressType type, Organisation organisation)
        {
            switch (type.Value)
            {
                case 1:
                    return organisation.OrganisationAddress.Id;
                    break;

                case 2:
                    return organisation.BusinessAddress.Id;
                    break;

                case 3:
                    return organisation.NotificationAddress.Id;
                    break;
                default:
                    throw new InvalidOperationException("No address id found.");
            }
        }
    }
}