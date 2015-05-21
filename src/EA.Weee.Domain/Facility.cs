namespace EA.Weee.Domain
{
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Facility : Entity
    {
        public Business Business { get; private set; }

        public bool IsActualSiteOfTreatment { get; private set; }

        public Address Address { get; private set; }

        public Contact Contact { get; private set; }

        public Country Country { get; private set; }

        protected Facility()
        {
        }

        public Facility(Business business, Address address, Contact contact, Country country, bool isActualSiteOfTreatment)
        {
            Guard.ArgumentNotNull(business);
            Guard.ArgumentNotNull(address);
            Guard.ArgumentNotNull(contact);
            Guard.ArgumentNotNull(country);

            this.Business = business;
            this.Address = address;
            this.Contact = contact;
            this.IsActualSiteOfTreatment = isActualSiteOfTreatment;
            this.Country = country;
        }
    }
}
