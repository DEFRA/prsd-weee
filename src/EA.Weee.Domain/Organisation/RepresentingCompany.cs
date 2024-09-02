namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using System;
    using System.ComponentModel.DataAnnotations;

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
            Guard.ArgumentNotNull(() => telephone, telephone);
            Guard.ArgumentNotNull(() => email, email);
            Guard.ArgumentNotNull(() => country, country);

            CompanyName = companyName;
            TradingName = tradingName;
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

        [StringLength(256)]
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

        [StringLength(256)]
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

        [StringLength(60)]
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

        [StringLength(60)]
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

        [StringLength(35)]
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

        [StringLength(35)]
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

        [StringLength(10)]
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

        [StringLength(20)]
        public string Telephone
        {
            get => telephone;
            private set
            {
                if (value.Length > 20)
                {
                    throw new InvalidOperationException(string.Format(("Telephone cannot be greater than 20 characters")));
                }
                telephone = value;
            }
        }

        [StringLength(256)]
        public string Email
        {
            get => email;
            private set
            {
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
    }
}
