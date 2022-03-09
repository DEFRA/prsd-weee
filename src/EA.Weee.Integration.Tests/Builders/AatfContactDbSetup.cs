namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;

    public class AatfContactDbSetup : DbTestDataBuilder<AatfContact, AatfContactDbSetup>
    {
        protected override AatfContact Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));

            instance = new AatfContact(Faker.Name.First(),
                Faker.Name.Last(),
                Faker.Lorem.GetFirstWord(),
                Faker.Address.StreetAddress(),
                Faker.Address.SecondaryAddress(),
                Faker.Address.City(),
                Faker.Address.UkCounty(),
                Faker.Address.UkPostCode(),
                country,
                Faker.Phone.Number(),
                Faker.Internet.Email());

            return instance;
        }
    }
}
