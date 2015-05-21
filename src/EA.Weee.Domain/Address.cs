namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address
    {
        public Address(string building, string address1, string address2, string townOrCity, string postalCode, string country)
        {
            Guard.ArgumentNotNull(building);
            Guard.ArgumentNotNull(townOrCity);
            Guard.ArgumentNotNull(postalCode);
            Guard.ArgumentNotNull(country);
            Guard.ArgumentNotNull(address1);

            Building = building;
            TownOrCity = townOrCity;
            PostalCode = postalCode;
            Country = country;
            Address2 = address2;
            Address1 = address1;
        }

        protected Address()
        {
        }

        public string Building { get; private set; }

        public string Address1 { get; private set; }

        public string Address2 { get; private set; }

        public string TownOrCity { get; private set; }

        public string PostalCode { get; private set; }

        public string Country { get; private set; }

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