namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EditAatfContactHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfContactDataAccess aatfContactDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditAatfContactHandler handler;

        public EditAatfContactHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            aatfContactDataAccess = A.Fake<IAatfContactDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            context = A.Fake<WeeeContext>();

            handler = new EditAatfContactHandler(context, authorization, aatfContactDataAccess, genericDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new EditAatfContactHandler(context, authorization, aatfContactDataAccess, genericDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfContact>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoAdminRoleAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().DenyRole(Roles.InternalAdmin).Build();

            var handler = new EditAatfContactHandler(context, authorization, aatfContactDataAccess, genericDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfContact>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAddress_DetailsAreUpdatedCorrectly()
        {
            var addressData = new AatfContactAddressData()
            {
                Address1 = "Address1",
                Address2 = "Address2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                Postcode = "Postcode",
                CountryId = Guid.NewGuid()
            };

            var updateRequest = new EditAatfContact()
            {
                ContactData = new AatfContactData()
                {
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Position = "Position",
                    AddressData = addressData,
                    Telephone = "01234 567890",
                    Email = "email@email.com",
                    Id = Guid.NewGuid()
                }
            };

            var returnContact = A.Fake<AatfContact>();

            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => organisationDetailsDataAccess.FetchCountryAsync(updateRequest.ContactData.AddressData.CountryId)).Returns(country);
            A.CallTo(() => genericDataAccess.GetById<AatfContact>(updateRequest.ContactData.Id)).Returns(returnContact);

            await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfContactDataAccess.Update(returnContact, updateRequest.ContactData, country)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
