namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Organisations;

    internal class ContactDataMap : IMap<Contact, ContactData>
    {
        public ContactData Map(Contact source)
        {
            return new ContactData
            {
                Title = source.Title,
                Firstname = source.FirstName,
                Lastname = source.LastName,
                Position = source.Position
            };
        }
    }
}