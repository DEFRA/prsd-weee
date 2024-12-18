namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetUserOrganisationTransactionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess transactionDataAccess;
        private readonly IJsonSerializer serializer;
        private readonly GetUserOrganisationTransactionHandler handler;

        public GetUserOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            transactionDataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            handler = new GetUserOrganisationTransactionHandler(
                authorization,
                transactionDataAccess,
                serializer);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new GetUserOrganisationTransaction();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var denyAuthorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var request = new GetUserOrganisationTransaction();
            var authHandler = new GetUserOrganisationTransactionHandler(
                denyAuthorization,
                transactionDataAccess,
                serializer);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_NoIncompleteTransaction_ReturnsNull()
        {
            // Arrange
            var request = new GetUserOrganisationTransaction();
            A.CallTo(() => transactionDataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(Task.FromResult<OrganisationTransaction>(null));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Null(result);
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_IncompleteTransactionExists_DeserializesAndReturnsData()
        {
            // Arrange
            var request = new GetUserOrganisationTransaction();
            var transaction = new OrganisationTransaction { OrganisationJson = "json_data" };
            var deserializedData = new OrganisationTransactionData();

            A.CallTo(() => transactionDataAccess.FindIncompleteTransactionForCurrentUserAsync()).Returns(Task.FromResult(transaction));
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(transaction.OrganisationJson)).Returns(deserializedData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Same(deserializedData, result);
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(transaction.OrganisationJson)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_ThrowsException_WhenAuthorizationFails()
        {
            // Arrange
            var request = new GetUserOrganisationTransaction();
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<UnauthorizedAccessException>();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(request));
        }
    }
}