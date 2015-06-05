namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class AddOrganisationContactDetails : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public AddressData OrganisationContactAddress { get; set; }
    }
}
