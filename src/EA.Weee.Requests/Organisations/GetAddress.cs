namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

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
