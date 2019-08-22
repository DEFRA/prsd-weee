namespace EA.Weee.Requests.Organisations
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class GetAddress : IRequest<AddressData>
    {
        public Guid AddressId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public GetAddress(Guid addressId, Guid organisationId)
        {
            AddressId = addressId;
            OrganisationId = organisationId;
        }
    }
}
