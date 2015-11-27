namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;
    public class AddContactPersonToOrganisation : IRequest<Guid>
    {
        public AddContactPersonToOrganisation(Guid id, ContactData contact)
        {
            OrganisationId = id;
            ContactPerson = contact;
        }

        public Guid OrganisationId { get; set; }

        public ContactData ContactPerson { get; set; }
    }
}
