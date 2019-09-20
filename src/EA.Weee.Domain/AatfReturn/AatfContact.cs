namespace EA.Weee.Domain.AatfReturn
{
    using System;

    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class AatfContact : Entity, IEquatable<AatfContact>
    {
        public AatfContact()
        {
        }

        public AatfContact(
            string firstName,
            string lastName,
            string position,
            string address1,
            string address2,
            string townOrCity,
            string countyOrRegion,
            string postcode,
            Country country,
            string telephone,
            string email)
        {
            Guard.ArgumentNotNullOrEmpty(() => firstName, firstName);
            Guard.ArgumentNotNullOrEmpty(() => lastName, lastName);
            Guard.ArgumentNotNullOrEmpty(() => position, position);
            Guard.ArgumentNotNullOrEmpty(() => address1, address1);
            Guard.ArgumentNotNullOrEmpty(() => townOrCity, townOrCity);
            Guard.ArgumentNotNullOrEmpty(() => telephone, telephone);
            Guard.ArgumentNotNullOrEmpty(() => email, email);

            this.FirstName = firstName;
            this.LastName = lastName;
            this.Position = position;
            this.Address1 = address1;
            this.Address2 = address2;
            this.TownOrCity = townOrCity;
            this.CountyOrRegion = countyOrRegion;
            this.Postcode = postcode;
            this.Country = country;
            this.Telephone = telephone;
            this.Email = email;
            this.CountryId = country.Id;
        }

        public AatfContact(
            string firstName,
            string lastName,
            string position,
            string address1,
            string address2,
            string townOrCity,
            string countyOrRegion,
            string postcode,
            Guid countryId,
            string telephone,
            string email)
        {
            Guard.ArgumentNotNullOrEmpty(() => firstName, firstName);
            Guard.ArgumentNotNullOrEmpty(() => lastName, lastName);
            Guard.ArgumentNotNullOrEmpty(() => position, position);
            Guard.ArgumentNotNullOrEmpty(() => address1, address1);
            Guard.ArgumentNotNullOrEmpty(() => townOrCity, townOrCity);
            Guard.ArgumentNotNullOrEmpty(() => telephone, telephone);
            Guard.ArgumentNotNullOrEmpty(() => email, email);

            this.FirstName = firstName;
            this.LastName = lastName;
            this.Position = position;
            this.Address1 = address1;
            this.Address2 = address2;
            this.TownOrCity = townOrCity;
            this.CountyOrRegion = countyOrRegion;
            this.Postcode = postcode;
            this.Telephone = telephone;
            this.Email = email;
            this.CountryId = countryId;
        }

        public virtual void UpdateDetails(
            string firstName,
            string lastName,
            string position,
            string address1,
            string address2,
            string townOrCity,
            string countyOrRegion,
            string postcode,
            Country country,
            string telephone,
            string email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Position = position;
            this.Address1 = address1;
            this.Address2 = address2;
            this.TownOrCity = townOrCity;
            this.CountyOrRegion = countyOrRegion;
            this.Postcode = postcode;
            this.Country = country;
            this.Telephone = telephone;
            this.Email = email;
        }

        public virtual string FirstName { get; private set; }

        public virtual string LastName { get; private set; }

        public virtual string Position { get; private set; }

        public virtual string Address1 { get; private set; }

        public virtual string Address2 { get; private set; }

        public virtual string TownOrCity { get; private set; }

        public virtual string CountyOrRegion { get; private set; }

        public virtual string Postcode { get; private set; }

        public virtual Guid CountryId { get; private set; }

        public virtual Country Country { get; private set; }

        public virtual string Telephone { get; private set; }

        public virtual string Email { get; private set; }

        public virtual bool Equals(AatfContact other)
        {
            if (other == null)
            {
                return false;
            }

            return this.FirstName == other.FirstName && this.LastName == other.LastName && this.Position == other.Position
                   && this.Address1 == other.Address1 && this.Address2 == other.Address2 && this.TownOrCity == other.TownOrCity
                   && this.CountyOrRegion == other.CountyOrRegion && this.Postcode == other.Postcode && this.CountryId == other.CountryId
                   && this.Telephone == other.Telephone && this.Email == other.Email;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as AatfContact);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}