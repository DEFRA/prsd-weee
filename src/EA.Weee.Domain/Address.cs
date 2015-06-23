namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address : Entity
    {
        public Address(string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Country country, string telephone, string email)
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
        private string telephone;
        private string email;

        public virtual Country Country { get; private set; }

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
                if (Country.Id.Equals(Guid.Parse("FE1E7E10-D8AA-47BD-B8B7-F2C5C43643F3"))
                    || Country.Id.Equals(Guid.Parse("50B51FED-A94D-415B-9692-6A381E6EACA9"))
                    || Country.Id.Equals(Guid.Parse("B592E944-F39D-427E-949E-A7E79D999A42"))
                    || Country.Id.Equals(Guid.Parse("9CBFED0C-3341-4FFD-BA24-87605D0BBDA7"))) 
                {
                    return true;
                }
                return false;
            }
        }
    }
}