namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;
    using AatfContact = Domain.AatfReturn.AatfContact;

    public class AatfContactIntegration
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        public AatfContactIntegration()
        {
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
        }

        [Fact]
        public async void UpdateDetails_GivenDetailsToUpdate_ContextShouldContainUpdatedDetails()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var aatfAddress = new AatfContact("FirstName", "LastName", "Position", "Address1", "Address2", "Town", "County", "PO12ST34", country, "Telephone", "Email");

                var dataAccess = new AatfDataAccess(context, new GenericDataAccess(database.WeeeContext), quarterWindowFactory);

                var aatfId = await CreateContact(database, aatfAddress);

                var oldContact = context.Aatfs.First(a => a.Id == aatfId).Contact;

                var newAddressData = new AatfContactAddressData(null, "Address11", "Address21", "Town1", "County1", "PO12ST341", country.Id, country.Name);
                var newContact = new AatfContactData(A.Dummy<Guid>(), "FirstName1", "LastName1", "Position1", newAddressData, "Telephone1", "Email1");
                var newCountry = await context.Countries.SingleAsync(c => c.Name == "Germany");

                await dataAccess.UpdateContact(oldContact, newContact, newCountry);

                AssertUpdated(context, aatfId, newContact, newCountry);
            }
        }

        private async Task<Guid> CreateContact(DatabaseWrapper database, AatfContact aatfAddress)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

            database.WeeeContext.Organisations.Add(organisation);
            database.WeeeContext.Schemes.Add(scheme);
            database.WeeeContext.AatfContacts.Add(aatfAddress);
            database.WeeeContext.Aatfs.Add(aatf);

            await database.WeeeContext.SaveChangesAsync();

            return (aatf.Id);
        }

        private static void AssertUpdated(WeeeContext context, Guid aatfId, AatfContactData newContact, Domain.Country newCountry)
        {
            var updatedContact = context.Aatfs.First(a => a.Id == aatfId).Contact;

            updatedContact.Should().NotBeNull();
            updatedContact.FirstName.Should().NotBeNullOrEmpty();
            updatedContact.FirstName.Should().Be(newContact.FirstName);
            updatedContact.LastName.Should().NotBeNullOrEmpty();
            updatedContact.LastName.Should().Be(newContact.LastName);
            updatedContact.Position.Should().NotBeNullOrEmpty();
            updatedContact.Position.Should().Be(newContact.Position);
            updatedContact.Address1.Should().NotBeNullOrEmpty();
            updatedContact.Address1.Should().Be(newContact.AddressData.Address1);
            updatedContact.Address2.Should().Be(newContact.AddressData.Address2);
            updatedContact.TownOrCity.Should().NotBeNullOrEmpty();
            updatedContact.TownOrCity.Should().Be(newContact.AddressData.TownOrCity);
            updatedContact.CountyOrRegion.Should().Be(newContact.AddressData.CountyOrRegion);
            updatedContact.Postcode.Should().Be(newContact.AddressData.Postcode);
            updatedContact.CountryId.Should().NotBeEmpty();
            updatedContact.CountryId.Should().Be(newCountry.Id);
            updatedContact.Telephone.Should().NotBeNullOrEmpty();
            updatedContact.Telephone.Should().Be(newContact.Telephone);
            updatedContact.Email.Should().NotBeNullOrEmpty();
            updatedContact.Email.Should().Be(newContact.Email);
        }
    }
}
