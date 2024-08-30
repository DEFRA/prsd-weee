namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Producer;
    using System;

    public class RepresentingCompany : Entity
    {
        public RepresentingCompany(string companyName, string tradingName, string address1, string address2, string townOrCity, string countyOrRegion, string postcode,
           Country country, string telephone, string email)
        {
            Guard.ArgumentNotNull(() => companyName, companyName);
            Guard.ArgumentNotNull(() => tradingName, tradingName);
            Guard.ArgumentNotNull(() => address1, address1);
            Guard.ArgumentNotNull(() => townOrCity, townOrCity);
            Guard.ArgumentNotNull(() => postcode, postcode);
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

        protected RepresentingCompany()
        {
        }

        private string companyName;
        private string tradingName;
        private string address1;
        private string address2;
        private string townOrCity;
        private string countyOrRegion;
        private string postcode;
        private string telephone;
        private string email;

        public virtual Country Country { get; protected set; }

        public virtual Guid CountryId { get; private set; }

        public string CompanyName
        {
            get => companyName;
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 256)
                {
                    throw new InvalidOperationException(string.Format(("Company name cannot be greater than 256 characters")));
                }
                companyName = value;
            }
        }

        public string TradingName
        {
            get => tradingName;
            private set
            {
                if (value != null && value.Length > 256)
                {
                    throw new InvalidOperationException(string.Format(("Trading name cannot be greater than 256 characters")));
                }
                tradingName = value;
            }
        }

        public string Address1
        {
            get => address1;
            private set
            {
                Guard.ArgumentNotNullOrEmpty(() => value, value);
                if (value.Length > 60)
                {
                    throw new InvalidOperationException(string.Format(("Address1 cannot be greater than 60 characters")));
                }
                address1 = value;
            }
        }

        public string Address2
        {
            get => address2;
            private set
            {
                if (value != null && value.Length > 60)
                {
                    throw new InvalidOperationException(string.Format(("Address2 cannot be greater than 60 characters")));
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
                if (value != null && value.Length > 20)
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
                if (value != null && value.Length > 256)
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
    }
}
