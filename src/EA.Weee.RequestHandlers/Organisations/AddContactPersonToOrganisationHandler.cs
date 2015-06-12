namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class AddContactPersonToOrganisationHandler : IRequestHandler<AddContactPersonToOrganisation, Guid>
    {
        private readonly WeeeContext db;

        public AddContactPersonToOrganisationHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<Guid> HandleAsync(AddContactPersonToOrganisation message)
        {
            var contactPerson = ValueObjectInitializer.CreateContact(message.ContactPerson);
            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);
            organisation.AddMainContactPerson(contactPerson);
            int x = await db.SaveChangesAsync();
            return organisation.Contact.Id;
        }
    }
}