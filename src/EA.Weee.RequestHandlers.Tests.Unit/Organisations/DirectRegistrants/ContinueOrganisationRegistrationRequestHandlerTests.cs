namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class ContinueOrganisationRegistrationRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer serializer;
        private readonly ContinueOrganisationRegistrationRequestHandler handler;
        private readonly Guid organisationId;

        public ContinueOrganisationRegistrationRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();
            organisationTransactionDataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            handler = new ContinueOrganisationRegistrationRequestHandler(authorization, smallProducerDataAccess,
                organisationTransactionDataAccess, serializer);
            organisationId = Guid.NewGuid();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            SetupValidDirectRegistrant();

            await handler.HandleAsync(request);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenOrganisationNotMigrated_ThrowsInvalidOperationException()
        {
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            SetupValidDirectRegistrant(npwdMigrated: false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenOrganisationMigrationComplete_ThrowsInvalidOperationException()
        {
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            SetupValidDirectRegistrant(npwdMigrated: true, npwdMigratedComplete: true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WithValidOrganisation_SerializesAndSavesData()
        {
            // Arrange
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            SetupValidDirectRegistrant();
            var serializedJson = "serialized-json";
            A.CallTo(() => serializer.Serialize(A<OrganisationTransactionData>._))
                .Returns(serializedJson);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => organisationTransactionDataAccess.UpdateOrCreateTransactionAsync(serializedJson))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WithValidOrganisation_ReturnsCorrectData()
        {
            // Arrange
            var request = new ContinueOrganisationRegistrationRequest(organisationId);
            SetupValidDirectRegistrant();

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.NpwdMigrated.Should().BeTrue();
            result.OrganisationViewModel.Should().NotBeNull();
            result.OrganisationViewModel.CompaniesRegistrationNumber.Should().Be("12345678");
            result.OrganisationViewModel.ProducerRegistrationNumber.Should().Be("WEE/AB1234CD");
            result.OrganisationViewModel.CompanyName.Should().Be("Test Company");
            result.OrganisationViewModel.BusinessTradingName.Should().Be("Trading Name");
        }

        private (DirectRegistrant, Organisation) SetupValidDirectRegistrant(
            bool npwdMigrated = true,
            bool npwdMigratedComplete = false)
        {
            var organisation = Organisation.CreateRegisteredCompany("Test Company", "12345678", "Trading Name");

            organisation.NpwdMigrated = npwdMigrated;
            organisation.NpwdMigratedComplete = npwdMigratedComplete;

            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.Organisation).Returns(organisation);
            A.CallTo(() => directRegistrant.ProducerRegistrationNumber).Returns("WEE/AB1234CD");

            A.CallTo(() => smallProducerDataAccess.GetDirectRegistrantByOrganisationId(organisationId))
                .Returns(Task.FromResult(directRegistrant));

            return (directRegistrant, organisation);
        }
    }
}