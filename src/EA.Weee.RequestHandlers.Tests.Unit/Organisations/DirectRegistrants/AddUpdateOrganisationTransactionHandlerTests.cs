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

    public class AddUpdateOrganisationTransactionHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess dataAccess;
        private readonly IJsonSerializer serializer;
        private readonly AddUpdateOrganisationTransactionHandler handler;

        public AddUpdateOrganisationTransactionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IOrganisationTransactionDataAccess>();
            serializer = A.Fake<IJsonSerializer>();
            handler = new AddUpdateOrganisationTransactionHandler(
                authorization,
                dataAccess,
                serializer);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new AddUpdateOrganisationTransaction(new OrganisationTransactionData());

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
            var request = new AddUpdateOrganisationTransaction(new OrganisationTransactionData());
            var authHandler = new AddUpdateOrganisationTransactionHandler(
                denyAuthorization,
                dataAccess,
                serializer);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await authHandler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_SerializesData_UpdatesOrCreatesTransaction_AndDeserializesResult()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData();
            var request = new AddUpdateOrganisationTransaction(transactionData);
            const string serializedData = "serialized_json_data";
            var updatedTransaction = new OrganisationTransaction { OrganisationJson = "updated_json_data" };
            var deserializedData = new OrganisationTransactionData();

            A.CallTo(() => serializer.Serialize(transactionData)).Returns(serializedData);
            A.CallTo(() => dataAccess.UpdateOrCreateTransactionAsync(serializedData)).Returns(updatedTransaction);
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(updatedTransaction.OrganisationJson)).Returns(deserializedData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => serializer.Serialize(transactionData)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dataAccess.UpdateOrCreateTransactionAsync(serializedData)).MustHaveHappenedOnceExactly();
            A.CallTo(() => serializer.Deserialize<OrganisationTransactionData>(updatedTransaction.OrganisationJson)).MustHaveHappenedOnceExactly();
            Assert.Same(deserializedData, result);
        }

        [Fact]
        public async Task HandleAsync_ThrowsException_WhenAuthorizationFails()
        {
            // Arrange
            var request = new AddUpdateOrganisationTransaction(new OrganisationTransactionData());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<UnauthorizedAccessException>();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(request));
        }
    }
}