namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address : Entity
    {
        public Address(string address1, string address2, string townOrCity, string countyOrRegion, string postalCode, string country, string telephone, string email)
        {
            Guard.ArgumentNotNull(address1);
            Guard.ArgumentNotNull(townOrCity);
            Guard.ArgumentNotNull(postalCode);
            Guard.ArgumentNotNull(country);
            Guard.ArgumentNotNull(telephone);
            Guard.ArgumentNotNull(email);

            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            PostalCode = postalCode;
            Country = country;
            TelePhone = telephone;
            Email = email;
        }

        protected Address()
        {
        }
        
        public string Address1 { get; private set; }

        public string Address2 { get; set; }

        public string TownOrCity { get; private set; }
        
        public string CountyOrRegion { get; set; }

        public string PostalCode { get; private set; }

        public string Country { get; private set; }

        public string TelePhone { get; private set; }

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