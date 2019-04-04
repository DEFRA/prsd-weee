namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class AddContactPersonToScheme : IRequest<Guid>
    {
        public AddContactPersonToScheme(Guid id, ContactData contact)
        {
            SchemeId = id;
            ContactPerson = contact;
        }

        public Guid SchemeId { get; set; }

        public ContactData ContactPerson { get; set; }
    }
}
