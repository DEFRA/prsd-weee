namespace EA.Weee.Requests.Organisations
{
    using Prsd.Core.Mediator;
    using System;
    using Shared;

    public class GetAddressByAddressType : IRequest<AddressData>
    {
        public Guid OrganisationsId { get; private set; }
        public AddressType AddressType { get; private set; }

        public GetAddressByAddressType(Guid organisationsId, AddressType addressType)
        {
            OrganisationsId = organisationsId;
            AddressType = addressType;
        }
    }
}
