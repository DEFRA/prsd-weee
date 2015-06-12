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
            this.db = context;
        }

        public async Task<Guid> HandleAsync(AddContactPersonToOrganisation command)
        {
            var contactPerson = ValueObjectInitializer.CreateContact(command.ContactPerson);
            var organisation = await db.Organisations.SingleAsync(o => o.Id == command.OrganisationId);
            organisation.AddMainContactPerson(contactPerson);
            await db.SaveChangesAsync();
            return contactPerson.Id;
        }
    }
}