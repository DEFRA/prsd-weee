﻿namespace EA.Weee.Domain.Organisation
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Address : Entity, IEquatable<Address>
    {
        public Address(string address1, string address2, string townOrCity, string countyOrRegion, string postcode,
            Country country, string telephone, string email)
        {
            Guard.ArgumentNotNull(() => country, country);

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

        public virtual Country Country { get; protected set; }

        public virtual Guid CountryId { get; private set; }

        public string Address1
        {
            get => address1;
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
            get => address2;
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
            get => townOrCity;
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 35)
                {
                    throw new InvalidOperationException(
                        string.Format(("Town Or City cannot be greater than 35 characters")));
                }
                townOrCity = value;
            }
        }

        public string CountyOrRegion
        {
            get => countyOrRegion;
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(
                        string.Format(("County Or Region cannot be greater than 35 characters")));
                }
                countyOrRegion = value;
            }
        }

        public string Postcode
        {
            get => postcode;
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
            get => telephone;
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
            get => email;
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

        public bool IsUkAddress()
        {
            if (Country != null)
            {
                return Country.Name.Contains("UK");
            }
            throw new InvalidOperationException("Country not defined.");
        }

        public Address OverwriteWhereNull(Address otherAddress)
        {
            if (otherAddress == null)
            {
                return this;
            }

            otherAddress.Address1 = Address1;
            otherAddress.Address2 = Address2;
            otherAddress.TownOrCity = TownOrCity;
            otherAddress.CountyOrRegion = CountyOrRegion;
            otherAddress.Postcode = Postcode;
            otherAddress.Country = Country;
            otherAddress.Telephone = Telephone;
            otherAddress.Email = Email;

            return otherAddress;
        }

        public void Overwrite(Address otherAddress)
        {
            if (otherAddress == null)
            {
                return;
            }

            this.Address1 = otherAddress.Address1;
            this.Address2 = otherAddress.Address2;
            this.TownOrCity = otherAddress.TownOrCity;
            this.CountyOrRegion = otherAddress.CountyOrRegion;
            this.Postcode = otherAddress.Postcode;
            this.Country = otherAddress.Country;
            this.Telephone = otherAddress.Telephone;
            this.Email = otherAddress.Email;
        }

        public bool Equals(Address other)
        {
            if (other == null)
            {
                return false;
            }

            return Address1 == other.Address1 &&
                   Address2 == other.Address2 &&
                   TownOrCity == other.TownOrCity &&
                   CountyOrRegion == other.CountyOrRegion &&
                   Postcode == other.Postcode &&
                   Telephone == other.Telephone &&
                   Email == other.Email &&
                   Equals(Country, other.Country);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Address);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
