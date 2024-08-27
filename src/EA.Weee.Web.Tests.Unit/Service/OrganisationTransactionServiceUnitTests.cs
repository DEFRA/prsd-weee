namespace EA.Weee.Web.Tests.Unit.Service
{
    using AutoFixture;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using FakeItEasy;
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationTransactionServiceTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly OrganisationTransactionService organisationService;
        private readonly Fixture fixture;

        public OrganisationTransactionServiceTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            IWeeeClient WeeeClientFactory() => weeeClient;
            organisationService = new OrganisationTransactionService(WeeeClientFactory);
            fixture = new Fixture();
        }

        [Fact]
        public async Task CaptureData_WithTonnageTypeViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var tonnageTypeViewModel = new TonnageTypeViewModel() { SelectedValue = "Less than 5 tonnes", SearchedText = "search" };
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, tonnageTypeViewModel);

            // Assert
            transaction.TonnageType.Should().Be(TonnageType.LessThanFiveTonnes);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.TonnageType.Equals(tonnageTypeViewModel.SelectedValue.GetValueFromDisplayName<TonnageType>()) &&
                                                x.OrganisationTransactionData.SearchTerm.Equals(tonnageTypeViewModel.SearchedText)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithOrganisationTypeViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var externalOrganisationTypeViewModel = new ExternalOrganisationTypeViewModel() { SelectedValue = "Partnership" };
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, externalOrganisationTypeViewModel);

            // Assert
            transaction.OrganisationType.Should().Be(ExternalOrganisationType.Partnership);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.OrganisationType.Equals(externalOrganisationTypeViewModel.SelectedValue.GetValueFromDisplayName<ExternalOrganisationType>())))).MustHaveHappenedOnceExactly();
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
            transaction.PreviousRegistration.Should().Be(YesNoType.Yes);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.PreviousRegistration.Equals(previousRegistrationModel.SelectedValue.GetValueFromDisplayName<YesNoType>())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithSoleTraderViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var soleTraderDetailsViewModel = fixture.Create<SoleTraderDetailsViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, soleTraderDetailsViewModel);

            // Assert
            transaction.SoleTraderDetailsViewModel.Should().Be(soleTraderDetailsViewModel);
            transaction.RegisteredCompanyDetailsViewModel.Should().BeNull();
            transaction.PartnershipDetailsViewModel.Should().BeNull();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.SoleTraderDetailsViewModel.Equals(soleTraderDetailsViewModel)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithRegisteredCompanyViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var registeredCompanyDetailsViewModel = fixture.Create<RegisteredCompanyDetailsViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, registeredCompanyDetailsViewModel);

            // Assert
            transaction.RegisteredCompanyDetailsViewModel.Should().Be(registeredCompanyDetailsViewModel);
            transaction.SoleTraderDetailsViewModel.Should().BeNull();
            transaction.PartnershipDetailsViewModel.Should().BeNull();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.Equals(registeredCompanyDetailsViewModel)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WitPartnerShipViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var partnershipDetailsViewModel = fixture.Create<PartnershipDetailsViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, partnershipDetailsViewModel);

            // Assert
            transaction.RegisteredCompanyDetailsViewModel.Should().BeNull();
            transaction.SoleTraderDetailsViewModel.Should().BeNull();
            transaction.PartnershipDetailsViewModel.Should().Be(partnershipDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.PartnershipDetailsViewModel.Equals(partnershipDetailsViewModel)))).MustHaveHappenedOnceExactly();
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
            transaction.RegisteredCompanyDetailsViewModel.Should().BeNull();
            transaction.OrganisationType.Should().BeNull();
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