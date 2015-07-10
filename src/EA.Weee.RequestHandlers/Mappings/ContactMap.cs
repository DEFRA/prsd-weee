namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Requests.Organisations;

    internal class ContactMap : IMap<Contact, ContactData>
    {
        public ContactData Map(Contact source)
        {
            return new ContactData
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position
            };
        }
    }
}
