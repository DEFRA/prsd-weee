namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class AddContactPerson : IRequest<Guid>
    {
        public AddContactPerson(Guid id, ContactData contact)
        {
            OrganisationId = id;
            ContactPerson = contact;
        }

        public Guid OrganisationId { get; set; }

        public ContactData ContactPerson { get; set; }
    }
}
