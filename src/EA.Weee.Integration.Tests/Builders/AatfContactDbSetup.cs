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

            instance = new AatfContact("aatf contact first",
                "aatf contact last",
                "Job",
                "Address 1",
                "Address 2",
                "London",
                "Greater London",
                Faker.Address.UkPostCode(),
                country,
                "01483 676934",
                "aatf@email.com");

            return instance;
        }
    }
}
