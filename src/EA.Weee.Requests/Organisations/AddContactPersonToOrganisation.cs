namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    public class AddContactPersonToOrganisation : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public ContactData MainContactPerson { get; set; }
    }
}
