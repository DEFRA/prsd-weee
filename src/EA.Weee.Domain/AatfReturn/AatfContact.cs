namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class AatfContact : Entity
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

            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            Country = country;
            Telephone = telephone;
            Email = email;
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
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            Country = country;
            Telephone = telephone;
            Email = email;
        }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Position { get; set; }

        public virtual string Address1 { get; private set; }

        public virtual string Address2 { get; private set; }

        public virtual string TownOrCity { get; private set; }

        public virtual string CountyOrRegion { get; private set; }

        public virtual string Postcode { get; private set; }

        public virtual Guid CountryId { get; private set; }

        public virtual Country Country { get; private set; }

        public virtual string Telephone { get; set; }

        public virtual string Email { get; set; }
    }
}