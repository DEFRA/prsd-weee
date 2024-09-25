namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using Domain.Organisation;
    using System;
    using System.Linq;

    public class AddressDbSetup : DbTestDataBuilder<Address, AddressDbSetup>
    {
        protected override Address Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));

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
    }
}
