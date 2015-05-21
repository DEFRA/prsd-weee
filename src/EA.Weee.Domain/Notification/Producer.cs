namespace EA.Weee.Domain.Notification
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Producer : Entity
    {
        public bool IsSiteOfExport { get; private set; }

        public Business Business { get; private set; }

        public Address Address { get; private set; }

        public Contact Contact { get; private set; }

        public Producer(Business business, Address address, Contact contact, bool isSiteOfExport)
        {
            Guard.ArgumentNotNull(business);
            Guard.ArgumentNotNull(address);
            Guard.ArgumentNotNull(contact);

            Business = business;
            Address = address;
            Contact = contact;
            IsSiteOfExport = isSiteOfExport;
        }

        protected Producer()
        {
        }

        public void SetSiteOfExport(bool isSiteOfExport)
        {
            IsSiteOfExport = isSiteOfExport;
        }
    }
}
