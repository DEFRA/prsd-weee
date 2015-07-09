namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System.Threading.Tasks;
    using Requests.Shared;
    using AddressType = Requests.Shared.AddressType;

    internal class GetAddressByAddressTypeHandler : IRequestHandler<GetAddressByAddressType, AddressData>
    {
        private readonly WeeeContext context;
        private readonly IMap<Address, AddressData> mapper;

        public GetAddressByAddressTypeHandler(WeeeContext context, IMap<Address, AddressData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<AddressData> HandleAsync(GetAddressByAddressType message)
        {
            var organisation = await context.Organisations.SingleAsync(n => n.Id == message.OrganisationsId);
            Address address = null;
            if (message.AddressType == AddressType.OrganistionAddress)
            {
                address = organisation.OrganisationAddress;
            }
            else if (message.AddressType == AddressType.RegisteredorPPBAddress)
            {
                address = organisation.BusinessAddress;
            }
            else if (message.AddressType == AddressType.ServiceOfNotice)
            {
                address = organisation.NotificationAddress;
            }

            var addressData = mapper.Map(address);

            addressData.HasOrganisationAddress = organisation.HasOrganisationAddress;
            addressData.HasBusinessAddress = organisation.HasBusinessAddress;
            addressData.HasNotificationAddress = organisation.HasNotificationAddress;

            return addressData;
        }
    }
}
