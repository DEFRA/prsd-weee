namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;

    public class AatfAddressDbSetup : DbTestDataBuilder<AatfAddress, AatfAddressDbSetup>
    {
        protected override AatfAddress Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));
            instance = new AatfAddress(Faker.Company.Name(),
                Faker.Address.StreetAddress(),
                Faker.Address.SecondaryAddress(),
                Faker.Address.City(),
                Faker.Address.UkCounty(),
                Faker.Address.UkPostCode(),
                country);

            return instance;
        }
    }
}
