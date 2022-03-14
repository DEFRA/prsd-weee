namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain.Organisation;

    public class OrganisationAddressDbSetup : DbTestDataBuilder<Address, OrganisationAddressDbSetup>
    {
        protected override Address Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));
            instance = new Address(Faker.Address.StreetAddress(),
                Faker.Address.SecondaryAddress(),
                Faker.Address.City(),
                Faker.Address.UkCounty(),
                Faker.Address.UkPostCode(),
                country,
                "01483 878787",
                Faker.Internet.Email());

            return instance;
        }
    }
}
