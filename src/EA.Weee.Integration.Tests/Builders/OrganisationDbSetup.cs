namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain;
    using Domain.Organisation;

    public class OrganisationDbSetup : DbTestDataBuilder<Organisation, OrganisationDbSetup>
    {
        protected override Organisation Instantiate()
        {
            var address = OrganisationAddressDbSetup.Init().Create();

            Instance = Organisation.CreateRegisteredCompany(Faker.Company.Name(), Faker.RandomNumber.Next(10000000, 999999999999999).ToString());

            var newAddress = DbContext.Addresses.First(a => a.Id.Equals(address.Id));
            
            Instance.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, newAddress);
            
            Instance.OrganisationStatus = OrganisationStatus.Complete;
         
            return Instance;
        }
    }
}
