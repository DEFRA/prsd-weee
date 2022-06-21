namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;

    public class AatfDbSetup : DbTestDataBuilder<Aatf, AatfDbSetup>
    {
        protected override Aatf Instantiate()
        {
            var auth = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
            var localArea = DbContext.LocalAreas.First(c => c.Name.Equals("Kent, South London and East Sussex (KSL)"));
            var panArea = DbContext.PanAreas.First(c => c.Name.Equals("South East"));
            var address = AatfAddressDbSetup.Init().Create();
            var contact = AatfContactDbSetup.Init().Create();

            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var newAddress = DbContext.AatfAddress.First(a => a.Id.Equals(address.Id));
            var newContact = DbContext.AatfContacts.First(c => c.Id.Equals(contact.Id));

            instance = new Aatf(Faker.Company.Name(),
                auth,
                $"WEE/AA{Faker.RandomNumber.Next(1000, 9999)}ZZ/ATF",
                AatfStatus.Approved,
                organisation,
                newAddress,
                AatfSize.Large,
                DateTime.Now,
                newContact,
                FacilityType.Aatf,
                DateTime.Now.Year,
                localArea,
                panArea);

            return instance;
        }

        public AatfDbSetup WithOrganisation(Guid organisationId)
        {
            instance.UpdateOrganisation(organisationId);

            return this;
        }
    }
}
