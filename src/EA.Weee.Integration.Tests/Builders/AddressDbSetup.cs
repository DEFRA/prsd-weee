namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using Domain.Organisation;
    using EA.Weee.Domain;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class AddressDbSetup : DbTestDataBuilder<Address, AddressDbSetup>
    {
        private Country country;

        protected override Address Instantiate()
        {
            //var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));
            var country = GetCountryByName("UK - England");
            var address1 = Faker.Address.StreetAddress();
            var address2 = Faker.Address.SecondaryAddress();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();

            instance = new Address(address1.Substring(0, Math.Min(address1.Length, 60)),
                address2.Substring(0, Math.Min(address2.Length, 60)),
                town.Substring(0, Math.Min(town.Length, 35)),
                county.Substring(0, Math.Min(county.Length, 35)),
                Faker.Address.UkPostCode(),
                country,
                "1234567",
                Faker.Internet.Email());

            return instance;
        }

        public AddressDbSetup WithCountry(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
            {
                throw new ArgumentException("Country name cannot be null or empty", nameof(countryName));
            }

            country = GetCountryByName(countryName);
            return this;
        }

        private Country GetCountryByName(string countryName)
        {
            // First check if the country is already being tracked
            var trackedCountry = DbContext.Set<Country>().Local
                .FirstOrDefault(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));

            if (trackedCountry != null)
            {
                return trackedCountry;
            }

            // If not in local, get from database
            var country = DbContext.Countries
                .AsNoTracking() // Important: Get without tracking
                .FirstOrDefault(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));

            if (country == null)
            {
                throw new InvalidOperationException($"Could not find country: {countryName}");
            }

            // Attach the country as unchanged
            DbContext.Set<Country>().Attach(country);
            DbContext.Entry(country).State = EntityState.Unchanged;

            return country;
        }
    }
}
