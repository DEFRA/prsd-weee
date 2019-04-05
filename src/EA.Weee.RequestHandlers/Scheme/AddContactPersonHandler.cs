namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    internal class AddContactPersonHandler : IRequestHandler<AddContactPerson, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public AddContactPersonHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(AddContactPerson message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            //CHECK ADD OR UPDATE?
            var contactPerson = ValueObjectInitializer.CreateContact(message.ContactPerson);

            db.Contacts.Add(contactPerson);

            await db.SaveChangesAsync();

            return contactPerson.Id;
        }
    }
}