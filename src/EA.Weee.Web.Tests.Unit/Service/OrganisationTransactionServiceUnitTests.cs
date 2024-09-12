namespace EA.Weee.Web.Tests.Unit.Service
{
    using AutoFixture;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using FakeItEasy;
    using FluentAssertions;
    using System.Linq;
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
            var externalOrganisationTypeViewModel = new OrganisationTypeViewModel() { SelectedValue = "Partnership" };
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
        public async Task CaptureData_WithOrganisationViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var organisationViewModel = fixture.Create<OrganisationViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, organisationViewModel);

            // Assert
            transaction.OrganisationViewModel.Should().Be(organisationViewModel);
            
            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.OrganisationViewModel.Equals(organisationViewModel)))).MustHaveHappenedOnceExactly();
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
            transaction.OrganisationViewModel.Should().BeNull();
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

        [Fact]
        public async Task CaptureData_WithAuthorisedRepresentativeViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var authorisedRepresentativeViewModel = new AuthorisedRepresentativeViewModel { SelectedValue = "Yes" };
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, authorisedRepresentativeViewModel);

            // Assert
            transaction.AuthorisedRepresentative.Should().Be(YesNoType.Yes);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.AuthorisedRepresentative.Equals(authorisedRepresentativeViewModel.SelectedValue.GetValueFromDisplayName<YesNoType>())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CompleteTransaction_ShouldCompleteTheTransaction()
        {
            // Arrange
            const string accessToken = "test-token";

            // Act
            await organisationService.CompleteTransaction(accessToken);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(accessToken,
                A<CompleteOrganisationTransaction>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithContactDetailsViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var contactDetailsViewModel = fixture.Create<ContactDetailsViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, contactDetailsViewModel);

            // Assert
            transaction.ContactDetailsViewModel.Should().Be(contactDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.ContactDetailsViewModel.Equals(contactDetailsViewModel)))).MustHaveHappenedOnceExactly();
        }
        [Fact]
        public async Task CaptureData_WithSoleTraderViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var soleTraderViewModel = fixture.Create<SoleTraderViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, soleTraderViewModel);

            // Assert
            transaction.SoleTraderViewModel.Should().Be(soleTraderViewModel);
            transaction.PartnerModels.Should().BeNull();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.SoleTraderViewModel.Equals(soleTraderViewModel) &&
                     x.OrganisationTransactionData.PartnerModels == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithRepresentingCompanyDetailsViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var representingCompanyDetailsViewModel = fixture.Create<RepresentingCompanyDetailsViewModel>();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, representingCompanyDetailsViewModel);

            // Assert
            transaction.RepresentingCompanyDetailsViewModel.Should().Be(representingCompanyDetailsViewModel);
            transaction.PartnerModels.Should().BeNull();
            transaction.SoleTraderViewModel.Should().BeNull();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                x => x.OrganisationTransactionData.RepresentingCompanyDetailsViewModel.Equals(representingCompanyDetailsViewModel) &&
                     x.OrganisationTransactionData.PartnerModels == null &&
                     x.OrganisationTransactionData.SoleTraderViewModel == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CaptureData_WithPartnerViewModel_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var partners = fixture.CreateMany<AdditionalContactModel>(2).ToList();
            var notRequiredPartners = fixture.CreateMany<NotRequiredPartnerModel>(1).ToList();
            var partnerViewModel = fixture.Build<PartnerViewModel>()
                .With(p => p.PartnerModels, partners)
                .With(p => p.NotRequiredPartnerModels, notRequiredPartners)
                .Create();

            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationService.CaptureData(accessToken, partnerViewModel);

            // Assert
            transaction.PartnerModels.Should().BeEquivalentTo(partnerViewModel.AllParterModels);
            transaction.SoleTraderViewModel.Should().BeNull();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<AddUpdateOrganisationTransaction>.That.Matches(
                    x => x.OrganisationTransactionData.PartnerModels != null &&
                         x.OrganisationTransactionData.PartnerModels.Count == partnerViewModel.AllParterModels.Count &&
                         partnerViewModel.AllParterModels.All(partner =>
                             x.OrganisationTransactionData.PartnerModels.Any(p =>
                                 p.FirstName == partner.FirstName && p.LastName == partner.LastName)))))
                .MustHaveHappenedOnceExactly();
        }
    }
}