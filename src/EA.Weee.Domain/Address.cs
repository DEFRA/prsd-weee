namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address : Entity
    {
        public Address(string address1, string address2, string townOrCity, string countyOrRegion, string postcode, string country, string telephone, string email)
        {
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
        
        private string address1;
        private string address2;
        private string townOrCity;
        private string countyOrRegion;
        private string postcode;
        private string country;
        private string telephone;
        private string email;

        public string Address1
        {
            get { return address1; }
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Address1 cannot be greater than 35 characters")));
                }
                address1 = value;
            }
        }

        public string Address2
        {
            get { return address2; }
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Address2 cannot be greater than 35 characters")));
                }
                address2 = value;
            }
        }
        
        public string TownOrCity
        {
            get { return townOrCity; }
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Town Or City cannot be greater than 35 characters")));
                }
                townOrCity = value;
            }
        }

        public string CountyOrRegion
        {
            get { return countyOrRegion; }
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("County Or Region cannot be greater than 35 characters")));
                }
                countyOrRegion = value;
            }
        }

        public string Postcode
        {
            get { return postcode; }
            private set
            {
                if (value != null && value.Length > 10)
                {
                    throw new InvalidOperationException(string.Format(("PostCode cannot be greater than 10 characters")));
                }
                postcode = value;
            }
        }

        public string Country
        {
            get { return country; }
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Country cannot be greater than 35 characters")));
                }
                country = value;
            }
        }

        public string Telephone
        {
            get { return telephone; }
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 20)
                {
                    throw new InvalidOperationException(string.Format(("Telephone cannot be greater than 20 characters")));
                }
                telephone = value;
            }
        }

        public string Email
        {
            get { return email; }
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 256)
                {
                    throw new InvalidOperationException(string.Format(("Email cannot be greater than 256 characters")));
                }
                email = value;
            }
        }

        public bool IsUkAddress
        {
            get
            {
                if (Country != null && IsUKRegion)
                {
                    return true;
                }

                return false;
            }
        }
      
        private bool IsUKRegion
        {
            get
            {
                if (Country != null && 
                    (Country.Equals("England", StringComparison.InvariantCultureIgnoreCase)
                    || 
                    Country.Equals("Wales", StringComparison.InvariantCultureIgnoreCase)
                    || 
                    Country.Equals("Northern Ireland", StringComparison.InvariantCultureIgnoreCase)
                    || 
                    Country.Equals("England", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
                return false;
            }
        }
    }
}