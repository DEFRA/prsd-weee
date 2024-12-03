namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Domain;
    using Domain.Organisation;
    using EA.Weee.Tests.Core;

    public class OrganisationDbSetup : DbTestDataBuilder<Organisation, OrganisationDbSetup>
    {
        protected override Organisation Instantiate()
        {
            var address = OrganisationAddressDbSetup.Init().Create();

            instance = Organisation.CreateRegisteredCompany(Faker.Company.Name(), Faker.RandomNumber.Next(10000000, 999999999).ToString(), Faker.Company.Name());

            var newAddress = DbContext.Addresses.First(a => a.Id.Equals(address.Id));
            
            instance.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, newAddress);
            
            instance.OrganisationStatus = OrganisationStatus.Complete;
         
            return instance;
        }

        public OrganisationDbSetup WithNpWdMigrated(bool npwdMigrated)
        {
            ObjectInstantiator<Organisation>.SetProperty(o => o.NpwdMigrated, npwdMigrated, instance);

            return this;
        }

        public OrganisationDbSetup WithNpWdMigratedComplete(bool npwdMigratedComplete)
        {
            ObjectInstantiator<Organisation>.SetProperty(o => o.NpwdMigratedComplete, npwdMigratedComplete, instance);

            return this;
        }
    }
}
