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
            instance = new AatfAddress("aatf address",
                "Address 1",
                "Address 2",
                "London",
                "Greater London",
                Faker.Address.UkPostCode(),
                country);

            return instance;
        }
    }
}
