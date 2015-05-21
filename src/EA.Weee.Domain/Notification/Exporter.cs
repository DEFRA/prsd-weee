namespace EA.Weee.Domain.Notification
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Exporter : Entity
    {
        public Business Business { get; private set; }

        public Address Address { get; private set; }

        public Contact Contact { get; private set; }

        public Exporter(Business business, Address address, Contact contact)
        {
            Guard.ArgumentNotNull(business);
            Guard.ArgumentNotNull(address);
            Guard.ArgumentNotNull(contact);

            Business = business;
            Address = address;
            Contact = contact;
        }

        protected Exporter()
        {
        }
    }
}