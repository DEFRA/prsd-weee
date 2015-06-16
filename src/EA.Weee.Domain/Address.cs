namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address : Entity
    {
        public Address(string address1, string address2, string townOrCity, string countyOrRegion, string postcode, string country, string telephone, string email)
        {
            Guard.ArgumentNotNull(address1);
            Guard.ArgumentNotNull(townOrCity);
            Guard.ArgumentNotNull(country);
            Guard.ArgumentNotNull(telephone);
            Guard.ArgumentNotNull(email);

            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            Postcode = postcode;
            Country = country;
            Telephone = telephone;
            CountyOrRegion = countyOrRegion;
            Email = email;
        }

        protected Address()
        {
        }
        
        public string Address1 { get; private set; }

        public string Address2 { get; private set; }

        public string TownOrCity { get; private set; }
        
        public string CountyOrRegion { get; private set; }

        public string Postcode { get; set; }

        public string Country { get; private set; }

        public string Telephone { get; private set; }

        public string Email { get; private set; }

        public bool IsUkAddress
        {
            get
            {
                if (Country == null
                    || !Country.Equals("United Kingdom",
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            }
        }
    }
}