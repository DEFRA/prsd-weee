namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain.Organisation;

    public class OrganisationContactDbSetup : DbTestDataBuilder<Contact, OrganisationContactDbSetup>
    {
        protected override Contact Instantiate()
        {
            instance = new Contact(Faker.Name.First(),
                Faker.Name.Last(),
                Faker.Company.CatchPhrase().Substring(0, 20));

            return instance;
        }
    }
}
