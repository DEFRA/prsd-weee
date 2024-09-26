namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using Domain.Organisation;
    using System;
    using System.Linq;

    public class ContactDbSetup : DbTestDataBuilder<Contact, ContactDbSetup>
    {
        protected override Contact Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));

            var firstName = Faker.Name.First();
            var lastName = Faker.Name.Last();
           
            instance = new Contact(firstName.Substring(0, Math.Min(firstName.Length, 35)),
                lastName.Substring(0, Math.Min(lastName.Length, 35)),
                "Job");

            return instance;
        }
    }
}
