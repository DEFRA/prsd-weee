namespace EA.Weee.Integration.Tests.Builders
{
    using System;
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

            var companyName = Faker.Company.Name();
            var address1 = Faker.Address.StreetAddress();
            var address2 = Faker.Address.SecondaryAddress();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();

            instance = new AatfAddress(companyName.Substring(0, Math.Min(companyName.Length, 256)),
                address1.Substring(0, Math.Min(address1.Length, 60)),
                address2.Substring(0, Math.Min(address2.Length, 60)),
                town.Substring(0, Math.Min(town.Length, 35)),
                county.Substring(0, Math.Min(county.Length, 35)),
                Faker.Address.UkPostCode(),
                country);

            return instance;
        }
    }
}
