namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Domain.Organisation;
    using Prsd.Core.Mapper;

    internal class ContactMap : IMap<Contact, ContactData>, IMap<Organisation, ContactData>
    {
        public ContactData Map(Contact source)
        {
            return new ContactData
            {
                RowVersion = source.RowVersion,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position,
                HasContact = true
            };
        }

        public ContactData Map(Organisation source)
        {
            if (source.HasContact)
            {
                return new ContactData
                {
                    RowVersion = source.Contact.RowVersion,
                    FirstName = source.Contact.FirstName,
                    LastName = source.Contact.LastName,
                    Position = source.Contact.Position,
                    OrganisationId = source.Id,
                    HasContact = true
                };
            }
            return new ContactData
            {
                OrganisationId = source.Id,
                HasContact = false
            };
        }
    }
}
