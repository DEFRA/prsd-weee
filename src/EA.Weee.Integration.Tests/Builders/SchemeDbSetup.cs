namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Domain.Obligation;
    using Domain.Organisation;
    using Domain.Scheme;

    public class SchemeDbSetup : DbTestDataBuilder<Scheme, SchemeDbSetup>
    {
        protected override Scheme Instantiate()
        {
            var auth = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
            var address = OrganisationAddressDbSetup.Init().Create();
            var contact = OrganisationContactDbSetup.Init().Create();

            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var newAddress = DbContext.Addresses.First(a => a.Id.Equals(address.Id));
            var newContact = DbContext.Contacts.First(c => c.Id.Equals(contact.Id));

            instance = new Scheme(organisation);
            instance.UpdateScheme(Faker.Lorem.GetFirstWord(), 
                $"WEE/TE{Faker.RandomNumber.Next(1000, 9999)}ST/SCH",
                Faker.RandomNumber.Next(1, 100000000).ToString(),
                ObligationType.B2B,
                auth);

            instance.SetStatus(SchemeStatus.Pending);
            instance.AddOrUpdateAddress(newAddress);
            instance.AddOrUpdateMainContactPerson(newContact);

            return instance;
        }

        public SchemeDbSetup WithStatus(SchemeStatus status)
        {
            instance.SetStatus(status);

            return this;
        }

        public SchemeDbSetup WithOrganisation(Guid id)
        {
            instance.UpdateOrganisation(id);

            return this;
        }
    }
}
