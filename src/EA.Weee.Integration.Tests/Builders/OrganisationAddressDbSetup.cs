namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Domain.Organisation;

    public class OrganisationAddressDbSetup : DbTestDataBuilder<Address, OrganisationAddressDbSetup>
    {
        protected override Address Instantiate()
        {
            var address1 = Faker.Address.StreetAddress();
            var address2 = Faker.Address.SecondaryAddress();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var phone = Faker.Phone.Number();

            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));
            instance = new Address(address1.Substring(0, Math.Min(address1.Length, 60)),
                address2.Substring(0, Math.Min(address2.Length, 60)),
                town.Substring(0, Math.Min(town.Length, 35)),
                county.Substring(0, Math.Min(county.Length, 35)),
                Faker.Address.UkPostCode(),
                country,
                phone.Substring(0, Math.Min(phone.Length, 20)),
                Faker.Internet.Email());

            return instance;
        }
    }
}
