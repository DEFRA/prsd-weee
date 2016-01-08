namespace EA.Weee.Tests.Core.Model
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Transactions;
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using FakeItEasy;

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
    /// will be automatically rolled back when the DatabaseWrapper is disposed.
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
            // Create test user(context auditing requires a valid user id)
            var userId = string.Empty;
            using (var model = new Entities())
            {
                var testUserName = "WeeeIntegrationTestUser";
                var testUser = model.AspNetUsers.FirstOrDefault(u => u.UserName == testUserName);

                if (testUser == null)
                {
                    testUser = new AspNetUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = "Test",
                        Surname = "LastName",
                        Email = "a@b.c",
                        EmailConfirmed = true,
                        UserName = testUserName
                    };

                    model.AspNetUsers.Add(testUser);
                    model.SaveChanges();
                }

                userId = testUser.Id;
            }

            transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Model = new Entities();

            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .ReturnsLazily(() => Guid.Parse(userId));

            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            WeeeContext = new WeeeContext(userContext, eventDispatcher);

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
