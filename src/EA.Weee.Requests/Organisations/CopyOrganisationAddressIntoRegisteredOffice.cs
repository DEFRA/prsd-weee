namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class CopyOrganisationAddressIntoRegisteredOffice : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public CopyOrganisationAddressIntoRegisteredOffice(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}