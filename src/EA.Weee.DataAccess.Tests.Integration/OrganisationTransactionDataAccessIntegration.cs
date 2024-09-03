namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationTransactionDataAccessTests : IDisposable
    {
        private readonly DatabaseWrapper database;
        private readonly IUserContext userContext;
        private readonly OrganisationTransactionDataAccess dataAccess;

        public OrganisationTransactionDataAccessTests()
        {
            database = new DatabaseWrapper();
            userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(Guid.Parse(database.WeeeContext.GetCurrentUser()));
            dataAccess = new OrganisationTransactionDataAccess(database.WeeeContext, userContext);
        }

        public void Dispose()
        {
            database.Dispose();
        }

        [Fact]
        public async Task FindIncompleteTransactionForCurrentUserAsync_ReturnsIncompleteTransaction()
        {
            // Arrange
            await CreateIncompleteTransactionAsync();

            // Act
            var result = await dataAccess.FindIncompleteTransactionForCurrentUserAsync();

            // Assert
            AssertTransactionDetails(result, CompletionStatus.Incomplete);
        }

        [Fact]
        public async Task UpdateOrCreateTransactionAsync_CreatesNewTransactionIfNoneExists()
        {
            // Act
            var result = await dataAccess.UpdateOrCreateTransactionAsync(GetTestOrganisationJson());

            // Assert
            AssertTransactionDetails(result, CompletionStatus.Incomplete, GetTestOrganisationJson());
        }

        [Fact]
        public async Task CompleteTransactionAsync_CompletesExistingTransaction()
        {
            // Arrange
            await CreateIncompleteTransactionAsync();
            var organisation = Domain.Organisation.Organisation.CreatePartnership("Trading name");
            
            // Act
            var result = await dataAccess.CompleteTransactionAsync(organisation);

            // Assert
            AssertTransactionDetails(result, CompletionStatus.Complete);
            result.Organisation.Should().Be(organisation);
        }

        [Fact]
        public async Task CompleteTransactionAsync_ThrowsExceptionWhenNoIncompleteTransactionExists()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                dataAccess.CompleteTransactionAsync(A.Fake<Domain.Organisation.Organisation>()));
        }

        private async Task CreateIncompleteTransactionAsync()
        {
            var incompleteTransaction = new OrganisationTransaction(userContext.UserId.ToString());
            database.WeeeContext.OrganisationTransactions.Add(incompleteTransaction);
            await database.WeeeContext.SaveChangesAsync();
        }

        private void AssertTransactionDetails(OrganisationTransaction transaction, CompletionStatus expectedStatus, string expectedJson = null)
        {
            transaction.Should().NotBeNull();
            transaction.UserId.Should().Be(userContext.UserId.ToString());
            transaction.CompletionStatus.Value.Should().Be(expectedStatus.Value);
            if (expectedJson != null)
            {
                transaction.OrganisationJson.Should().Be(expectedJson);
            }
        }

        private static string GetTestOrganisationJson(string name = "Test Organisation")
        {
            return $"{{ \"name\": \"{name}\" }}";
        }
    }
}