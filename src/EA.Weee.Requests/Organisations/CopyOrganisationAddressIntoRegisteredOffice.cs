namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class CopyOrganisationAddressIntoRegisteredOffice : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public Guid AddressId { get; private set; }

        public CopyOrganisationAddressIntoRegisteredOffice(Guid organisationId, Guid addressId)
        {
            OrganisationId = organisationId;
            AddressId = addressId;
        }
    }
}