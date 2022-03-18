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
            instance = new Address("Org Address 1",
                "Org Address 2",
                "Org City",
                "Org London",
                Faker.Address.UkPostCode(),
                country,
                "01483 878787",
                "org.email@email.com");

            return instance;
        }
    }
}
