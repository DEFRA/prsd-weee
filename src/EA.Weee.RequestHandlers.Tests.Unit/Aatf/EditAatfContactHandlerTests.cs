namespace EA.Weee.RequestHandlers.Tests.Unit.Aatf
{
    using System;
    using System.Security;
    using System.Threading.Tasks;

    using AutoFixture;
    using DataAccess.DataAccess;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Events;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class EditAatfContactHandlerTests
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        private readonly Fixture fixture;
        private readonly EditAatfContactHandler handler;

        public EditAatfContactHandlerTests()
        {
            this.aatfDataAccess = A.Fake<IAatfDataAccess>();
            this.genericDataAccess = A.Fake<IGenericDataAccess>();
            this.organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            fixture = new Fixture();

            this.handler = new EditAatfContactHandler(new AuthorizationBuilder().AllowEverything().Build(), this.aatfDataAccess, this.genericDataAccess, this.organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationOrInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalOrOrganisationAccess().Build();

            var handler = new EditAatfContactHandler(authorization, this.aatfDataAccess, this.genericDataAccess, this.organisationDetailsDataAccess);

            var request = this.fixture.Create<EditAatfContact>();

            A.CallTo(() => this.aatfDataAccess.GetDetails(request.AatfId)).Returns(A.Fake<Aatf>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoAdminRoleAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().DenyRole(Roles.InternalAdmin).Build();

            var handler = new EditAatfContactHandler(authorization, this.aatfDataAccess, this.genericDataAccess, this.organisationDetailsDataAccess);

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

            var updateRequest = new EditAatfContact(
                Guid.NewGuid(),
                new AatfContactData()
                    {
                        FirstName = "First Name",
                        LastName = "Last Name",
                        Position = "Position",
                        AddressData = addressData,
                        Telephone = "01234 567890",
                        Email = "email@email.com",
                        Id = Guid.NewGuid()
                    });

            var returnContact = A.Fake<AatfContact>();

            var country = new Country(A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => this.organisationDetailsDataAccess.FetchCountryAsync(updateRequest.ContactData.AddressData.CountryId)).Returns(country);
            A.CallTo(() => this.genericDataAccess.GetById<AatfContact>(updateRequest.ContactData.Id)).Returns(returnContact);

            await this.handler.HandleAsync(updateRequest);

            A.CallTo(() => this.aatfDataAccess.UpdateContact(returnContact, updateRequest.ContactData, country)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenExternalUpdateWhereAatfIsNotTheLatestRecord_InvalidOperationExpected()
        {
            var request = this.fixture.Build<EditAatfContact>().With(e => e.SendNotification, true).Create();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => this.aatfDataAccess.GetDetails(request.AatfId)).Returns(aatf);
            A.CallTo(() => this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId)).Returns(false);

            var exception = await Record.ExceptionAsync(() => this.handler.HandleAsync(request));

            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfContactHasChangeAndSendNotification_AatfContactDetailsUpdateEventShouldBeRaised()
        {
            var request = this.fixture.Build<EditAatfContact>().With(e => e.SendNotification, true).Create();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => this.aatfDataAccess.GetDetails(request.AatfId)).Returns(aatf);
            A.CallTo(() => this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId)).Returns(true);

            var result = await this.handler.HandleAsync(request);

            A.CallTo(() => aatf.RaiseEvent(A<AatfContactDetailsUpdateEvent>.That.Matches(a => Equals(a.Aatf, aatf)))).MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfContactHasChangeAndSendNotificationIsFalse_AatfContactDetailsUpdateEventShouldNotBeRaised()
        {
            var request = this.fixture.Build<EditAatfContact>().With(e => e.SendNotification, false).Create();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => this.aatfDataAccess.GetDetails(request.AatfId)).Returns(aatf);
            A.CallTo(() => this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId)).Returns(true);

            var result = await this.handler.HandleAsync(request);

            A.CallTo(() => aatf.RaiseEvent(A<AatfContactDetailsUpdateEvent>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfContactHasNotChangedAndSendNotificationIsTrue_AatfContactDetailsUpdateEventShouldNotBeRaised()
        {
            var request = this.fixture.Build<EditAatfContact>().With(e => e.SendNotification, false).Create();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Organisation).Returns(A.Fake<Organisation>());
            A.CallTo(() => this.aatfDataAccess.GetDetails(request.AatfId)).Returns(aatf);
            A.CallTo(() => this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId)).Returns(true);
            A.CallTo(() => aatf.Contact.Equals(A<AatfContact>._)).Returns(true);

            var result = await this.handler.HandleAsync(request);

            A.CallTo(() => aatf.RaiseEvent(A<AatfContactDetailsUpdateEvent>._)).MustNotHaveHappened();
        }
    }
}
