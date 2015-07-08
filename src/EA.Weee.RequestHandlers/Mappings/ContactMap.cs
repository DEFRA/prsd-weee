namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using EA.Weee.Requests.Organisations;
    using Prsd.Core.Mapper;
    using Requests;

    internal class ContactMap : IMap<Contact, ContactData>, IMap<Organisation, ContactData>
    {
        public ContactData Map(Contact source)
        {
            return new ContactData
            {
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
                    FirstName = source.Contact.FirstName,
                    LastName = source.Contact.LastName,
                    Position = source.Contact.Position,
                    OrganisationId = source.Id,
                    HasContact = true
                };
            }
            else
            {
                return new ContactData
                {
                    OrganisationId = source.Id,
                    HasContact = true
                };
            }
        }
    }
}
