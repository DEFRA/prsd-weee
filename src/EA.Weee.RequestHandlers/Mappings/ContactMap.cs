namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using EA.Weee.Requests.Organisations;
    using Prsd.Core.Mapper;
    using Requests;

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
