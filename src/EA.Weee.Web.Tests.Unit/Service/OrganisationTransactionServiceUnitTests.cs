namespace EA.Weee.Web.Tests.Unit.Service
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationTransactionServiceTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly OrganisationTransactionService organisationService;

        public OrganisationTransactionServiceTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            IWeeeClient WeeeClientFactory() => weeeClient;
            organisationService = new OrganisationTransactionService(WeeeClientFactory);
        }

        [Fact]
        public async Task CaptureData_WithPreviousRegistrationViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var previousRegistrationModel = new PreviousRegistrationViewModel { SelectedValue = "Yes" };
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, previousRegistrationModel);

            // Assert
            transaction.PreviousRegistration.Should().Be(previousRegistrationModel.SelectedValue);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.PreviousRegistration == previousRegistrationModel.SelectedValue))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithNonMatchingModel_ShouldNotUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var nonMatchingModel = new object();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, nonMatchingModel);

            // Assert
            transaction.OrganisationDetails.Should().BeNull();
            transaction.PreviousRegistration.Should().BeNull();
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.Equals(transaction)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WhenGetUserOrganisationTransactionReturnsNull_ShouldCreateNewTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult<OrganisationTransactionData>(null));

            // Act
            await organisationService.CaptureData(accessToken, new object());

            // Assert
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData != null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_ShouldDisposeWeeeClient()
        {
            // Arrange
            const string accessToken = "test-token";
            var organisationDetails = new OrganisationDetails();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(new OrganisationTransactionData()));

            // Act
            await organisationService.CaptureData(accessToken, organisationDetails);

            // Assert
            A.CallTo(() => weeeClient.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetOrganisationTransactionData_WhenTransactionExists_ShouldReturnTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var expectedTransaction = new OrganisationTransactionData();
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(expectedTransaction));

            // Act
            var result = await organisationService.GetOrganisationTransactionData(accessToken);

            // Assert
            result.Should().Be(expectedTransaction);
            A.CallTo(() => weeeClient.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetOrganisationTransactionData_WhenTransactionDoesNotExist_ShouldReturnNewTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult<OrganisationTransactionData>(null));

            // Act
            var result = await organisationService.GetOrganisationTransactionData(accessToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrganisationTransactionData>();
            A.CallTo(() => weeeClient.Dispose()).MustHaveHappenedOnceExactly();
        }
    }
}