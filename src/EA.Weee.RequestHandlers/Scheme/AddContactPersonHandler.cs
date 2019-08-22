﻿namespace EA.Weee.RequestHandlers.Scheme
{
    using AatfReturn;
    using DataAccess;
    using Domain.Organisation;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class AddContactPersonHandler : IRequestHandler<AddContactPerson, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;

        public AddContactPersonHandler(WeeeContext context, IWeeeAuthorization authorization, IGenericDataAccess dataAccess)
        {
            db = context;
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<Guid> HandleAsync(AddContactPerson message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var contactPerson = ValueObjectInitializer.CreateContact(message.ContactPerson);

            Guid id;
            if (message.ContactId.HasValue)
            {
                var contact = await dataAccess.GetById<Contact>(message.ContactId.Value);

                contact.Overwrite(contactPerson);

                id = contact.Id;
            }
            else
            {
                id = await dataAccess.Add<Contact>(contactPerson);
            }

            await db.SaveChangesAsync();

            return id;
        }
    }
}