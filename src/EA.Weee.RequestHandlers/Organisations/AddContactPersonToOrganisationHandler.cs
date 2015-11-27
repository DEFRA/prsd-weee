namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class AddContactPersonToOrganisationHandler : IRequestHandler<AddContactPersonToOrganisation, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public AddContactPersonToOrganisationHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(AddContactPersonToOrganisation message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var contactPerson = ValueObjectInitializer.CreateContact(message.ContactPerson);
            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);
            organisation.AddOrUpdateMainContactPerson(contactPerson);
            await db.SaveChangesAsync();
            return organisation.Contact.Id;
        }
    }
}