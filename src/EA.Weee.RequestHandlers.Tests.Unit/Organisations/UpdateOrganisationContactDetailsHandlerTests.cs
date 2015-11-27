namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Organisations;
    using FakeItEasy;
    using Xunit;

    public class UpdateOrganisationContactDetailsHandlerTests
    {
        /// <summary>
        /// This test ensures that the handler correctly updates each of the properties for the contact and organisation
        /// address of the specified organisation, before saving the changes.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithValidData_FetchesOrganisationAndUpdatesAndSaves()
        {
            // Arrange
            OrganisationData organisationData = new OrganisationData();
            organisationData.Id = new Guid("93646500-85A1-4F9D-AE18-73265426EF40");
            organisationData.Contact = new ContactData();
            organisationData.Contact.FirstName = "FirstName";
            organisationData.Contact.LastName = "LastName";
            organisationData.Contact.Position = "Position";
            organisationData.OrganisationAddress = new Core.Shared.AddressData();
            organisationData.OrganisationAddress.Address1 = "Address1";
            organisationData.OrganisationAddress.Address2 = "Address2";
            organisationData.OrganisationAddress.TownOrCity = "Town";
            organisationData.OrganisationAddress.CountyOrRegion = "County";
            organisationData.OrganisationAddress.Postcode = "Postcode";
            organisationData.OrganisationAddress.CountryId = new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC");
            organisationData.OrganisationAddress.Telephone = "012345678";
            organisationData.OrganisationAddress.Email = "email@domain.com";

            UpdateOrganisationContactDetails request = new UpdateOrganisationContactDetails(organisationData);

            IUpdateOrganisationContactDetailsDataAccess dataAccess = A.Fake<IUpdateOrganisationContactDetailsDataAccess>();

            Organisation organisation = A.Dummy<Organisation>();
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("93646500-85A1-4F9D-AE18-73265426EF40")))
                .Returns(organisation);

            Country country = new Country(
                new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC"),
                "Name");
            A.CallTo(() => dataAccess.FetchCountryAsync(new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC")))
                .Returns(country);

            UpdateOrganisationContactDetailsHandler handler = new UpdateOrganisationContactDetailsHandler(dataAccess);

            // Act
            bool result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => dataAccess.FetchOrganisationAsync(new Guid("93646500-85A1-4F9D-AE18-73265426EF40")))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal("FirstName", organisation.Contact.FirstName);
            Assert.Equal("LastName", organisation.Contact.LastName);
            Assert.Equal("Position", organisation.Contact.Position);
            Assert.Equal("Address1", organisation.OrganisationAddress.Address1);
            Assert.Equal("Address2", organisation.OrganisationAddress.Address2);
            Assert.Equal("Town", organisation.OrganisationAddress.TownOrCity);
            Assert.Equal("County", organisation.OrganisationAddress.CountyOrRegion);
            Assert.Equal("Postcode", organisation.OrganisationAddress.Postcode);
            Assert.Equal(new Guid("1AF4BB2F-D2B0-41EA-BFD8-B83764C1ECBC"), organisation.OrganisationAddress.Country.Id);
            Assert.Equal("012345678", organisation.OrganisationAddress.Telephone);
            Assert.Equal("email@domain.com", organisation.OrganisationAddress.Email);

            A.CallTo(() => dataAccess.SaveAsync())
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
