namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class CompleteOrganisationTransactionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess dataAccess;
        private readonly IJsonSerializer serializer;
        private readonly CompleteOrganisationTransactionHandler handler;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public CompleteOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            transactionAdapter = A.Fake<IWeeeTransactionAdapter>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new CompleteOrganisationTransactionHandler(
                authorization,
                dataAccess,
                serializer,
                transactionAdapter,
                genericDataAccess,
                weeeContext);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction(new OrganisationTransactionData());

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
            var request = new CompleteOrganisationTransaction(new OrganisationTransactionData());

            var authHandler = new CompleteOrganisationTransactionHandler(
                authorization,
                dataAccess,
                serializer,
                transactionAdapter,
                genericDataAccess,
                weeeContext);

            // Act & Assert
            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_SerializesData_AndCompleteTransaction()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData();
            var request = new CompleteOrganisationTransaction(transactionData);
            const string serializedData = "serialized_json_data";

            A.CallTo(() => serializer.Serialize(transactionData)).Returns(serializedData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => serializer.Serialize(transactionData)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.CompleteTransactionAsync(serializedData)).MustHaveHappenedOnceExactly();
            Assert.True(result);
        }

        [Fact]
        public async Task HandleAsync_ThrowsException_WhenAuthorizationFails()
        {
            // Arrange
            var request = new CompleteOrganisationTransaction(new OrganisationTransactionData());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<UnauthorizedAccessException>();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(request));
        }
    }
}