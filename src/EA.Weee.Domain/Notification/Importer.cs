namespace EA.Weee.Domain.Notification
{
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Importer : Entity
    {
        public Business Business { get; private set; }

        public Address Address { get; private set; }

        public Contact Contact { get; private set; }

        public Importer(Business business, Address address, Contact contact)
        {
            Guard.ArgumentNotNull(business);
            Guard.ArgumentNotNull(address);
            Guard.ArgumentNotNull(contact);

            Business = business;
            Address = address;
            Contact = contact;
        }

        protected Importer()
        {
        }
    }
}
