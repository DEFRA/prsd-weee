namespace EA.Weee.Tests.Core.Model
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using FakeItEasy;
    using System;
    using System.Data.Common;
    using System.Transactions;

    /// <summary>
    /// Provides a disposable wrapper for integration tests against a real
    /// database.
    /// 
    /// The Model provides raw access to the underlying database.
    /// It should be used to arrange (seed) the database and to assert inserts / updates.
    /// 
    /// The WeeeContext and StoredProcedures provide the application's view of the database, mapped
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
    ///     db.Model.Organsiations.Add(new Organisation { Id = 7 });
    ///     db.SaveChanges();
    /// 
    ///     // Act
    ///     IOrganisationDataAcess dataAccess = new OrganisationDataAcess(db.WeeeContext);
    ///     var result = dataAccess.FetchOrganisationData(7);
    ///     
    ///     // Assert
    ///     Assert.NotNull(result);
    ///     Assert.Equal(7, result);
    /// }
    /// </example>
    public class DatabaseWrapper : IDisposable
    {
        public Entities Model { get; private set; }

        public WeeeContext WeeeContext { get; private set; }

        public IStoredProcedures StoredProcedures { get; private set; }

        private TransactionScope transactionScope;

        public DatabaseWrapper()
        {
            transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Model = new Entities();
            DbConnection connection = Model.Database.Connection;

            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            WeeeContext = new WeeeContext(userContext, eventDispatcher, connection);

            StoredProcedures = new StoredProcedures(WeeeContext);
        }

        public void Dispose()
        {
            if (transactionScope != null)
            {
                transactionScope.Dispose();
                transactionScope = null;
            }

            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }

            if (WeeeContext != null)
            {
                WeeeContext.Dispose();
                WeeeContext = null;
            }
        }
    }
}
