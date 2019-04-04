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

    internal class AddContactPersonToSchemeHandler : IRequestHandler<AddContactPersonToScheme, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public AddContactPersonToSchemeHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(AddContactPersonToScheme message)
        {
            authorization.EnsureOrganisationAccess(message.SchemeId);

            var contactPerson = ValueObjectInitializer.CreateContact(message.ContactPerson);
            var scheme = await db.Schemes.SingleAsync(o => o.Id == message.SchemeId);
            scheme.AddOrUpdateMainContactPerson(contactPerson);
            await db.SaveChangesAsync();
            return scheme.Contact.Id;
        }
    }
}