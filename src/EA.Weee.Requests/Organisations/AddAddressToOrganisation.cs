namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class AddAddressToOrganisation : IRequest<Guid>
    {
        public AddAddressToOrganisation(Guid id, AddressType type, AddressData address)
        {
            OrganisationId = id;
            TypeOfAddress = type;
            Address = address;
        }

        public Guid OrganisationId { get; set; }

        public AddressData Address { get; set; }

        public AddressType TypeOfAddress { get; set; }
    }
}
