namespace EA.Prsd.Core.DataAccess.Tests.Integration.Helpers
{
    using System;
    using System.Data.Common;
    using System.Transactions;
    using DataAccess.Tests.Integration;
    using Model;
    using EA.Prsd.Core.Domain;
    using FakeItEasy;

    /// <summary>
    /// Provides a disposable wrapper for integration tests against a real test
    /// database.
    /// 
    /// The TestDbContext provides the application's view of the database, mapped
    /// to domain entities or result types.
    /// It should be used to act on the database.
    /// 
    /// Any changes made to the database will happen within a transaction which
    /// will never be automatically rolled back when the DatabaseWrapper is disposed.
    /// </summary>
    /// <example>
    /// using (DatabaseWrapper db = new DatabseWrapper())
    /// {
    ///     // Arrange
    ///     db.Model.SimpleEntity.Add(new SimpleEntity { Id = 7 });
    ///     db.SaveChanges();
    /// 
    ///     // Act
    ///     
    ///     // Assert
    /// 
    /// }
    /// </example>
    public class DatabaseWrapper : IDisposable
    {
        public TestDbContext TestContext { get; private set; }

        private TransactionScope transactionScope;

        public DatabaseWrapper()
        {
            transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            TestContext = new TestDbContext();

            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());
        }

        public void Dispose()
        {
            if (transactionScope != null)
            {
                transactionScope.Dispose();
                transactionScope = null;
            }

            if (TestContext != null)
            {
                TestContext.Dispose();
                TestContext = null;
            }
        }
    }
}
