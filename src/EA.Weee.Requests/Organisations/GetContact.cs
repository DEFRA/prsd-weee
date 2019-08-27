namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;

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
