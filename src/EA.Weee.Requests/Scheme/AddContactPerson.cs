namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class AddContactPerson : IRequest<Guid>
    {
        public AddContactPerson(Guid organisationId, ContactData contact, Guid? contactId)
        {
            OrganisationId = organisationId;
            ContactPerson = contact;
            ContactId = contactId;
        }

        public Guid OrganisationId { get; set; }

        public Guid? ContactId { get; set; }

        public ContactData ContactPerson { get; set; }
    }
}
