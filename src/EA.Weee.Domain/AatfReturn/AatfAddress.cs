namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class AatfAddress : Entity
    {
        public virtual string Name { get; private set; }

        public virtual string Address1 { get; private set; }

        public virtual string Address2 { get; private set; }

        public virtual string TownOrCity { get; private set; }

        public virtual string CountyOrRegion { get; private set; }

        public virtual string Postcode { get; private set; }

        public virtual Guid CountryId { get; private set; }

        public virtual Country Country { get; private set; }

        public AatfAddress()
        {
        }

        public AatfAddress(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Country country)
        {
            Guard.ArgumentNotNullOrEmpty(() => name, name);
            Guard.ArgumentNotNullOrEmpty(() => address1, address1);
            Guard.ArgumentNotNullOrEmpty(() => townOrCity, townOrCity);

            Name = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            Country = country;
            CountryId = country.Id;
        }

        public AatfAddress(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId)
        {
            Guard.ArgumentNotNullOrEmpty(() => name, name);
            Guard.ArgumentNotNullOrEmpty(() => address1, address1);
            Guard.ArgumentNotNullOrEmpty(() => townOrCity, townOrCity);

            Name = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            CountryId = countryId;
        }

        public virtual void UpdateAddress(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Country country)
        {
            Name = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            Country = country;
        }
    }
}
