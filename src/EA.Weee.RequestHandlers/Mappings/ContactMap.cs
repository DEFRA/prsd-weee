﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    internal class ContactMap : IMap<Contact, ContactData>, IMap<Scheme, ContactData>
    {
        public ContactData Map(Contact source)
        {
            return new ContactData
            {
                RowVersion = source.RowVersion,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position,
                HasContact = true,
                Id = source.Id
            };
        }

        public ContactData Map(Scheme source)
        {
            if (source.HasContact)
            {
                return new ContactData
                {
                    RowVersion = source.Contact.RowVersion,
                    FirstName = source.Contact.FirstName,
                    LastName = source.Contact.LastName,
                    Position = source.Contact.Position,
                    OrganisationId = source.OrganisationId,
                    HasContact = true,
                    Id = source.Id
                };
            }
            return new ContactData
            {
                OrganisationId = source.OrganisationId,
                HasContact = false
            };
        }
    }
}
