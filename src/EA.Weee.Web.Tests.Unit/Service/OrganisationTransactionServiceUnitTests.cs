namespace EA.Weee.Web.Tests.Unit.Service
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationTransactionServiceTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly Func<IWeeeClient> weeeClientFactory;
        private readonly OrganisationTransactionService organisationTransactionService;

        public OrganisationTransactionServiceTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            weeeClientFactory = () => weeeClient;
            organisationTransactionService = new OrganisationTransactionService(weeeClientFactory);
        }

        [Fact]
        public async Task CaptureData_WithOrganisationDetails_ShouldUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var organisationDetails = new OrganisationDetails();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationTransactionService.CaptureData(accessToken, organisationDetails);

            // Assert
            Assert.Same(organisationDetails, transaction.OrganisationDetails);
        }

        [Fact]
        public async Task CaptureData_WithNonOrganisationDetailsModel_ShouldNotUpdateTransaction()
        {
            // Arrange
            const string accessToken = "test-token";
            var nonOrganisationDetails = new object();
            var transaction = new OrganisationTransactionData();

            A.CallTo(() => weeeClient.SendAsync(accessToken, A<GetUserOrganisationTransaction>.Ignored))
                .Returns(Task.FromResult(transaction));

            // Act
            await organisationTransactionService.CaptureData(accessToken, nonOrganisationDetails);

            // Assert
            Assert.Null(transaction.OrganisationDetails);
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
            await organisationTransactionService.CaptureData(accessToken, organisationDetails);

            // Assert
            A.CallTo(() => weeeClient.Dispose()).MustHaveHappenedOnceExactly();
        }
    }
}