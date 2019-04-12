namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetContact : IRequest<ContactData>
    {
        public Guid ContactId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public GetContact(Guid contactId, Guid organisationId)
        {
            ContactId = contactId;
            OrganisationId = organisationId;
        }
    }
}
