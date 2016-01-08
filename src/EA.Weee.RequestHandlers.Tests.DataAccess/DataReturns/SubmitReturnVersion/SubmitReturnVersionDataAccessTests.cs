namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.SubmitReturnVersion
{
    using System.Threading.Tasks;
    using RequestHandlers.DataReturns.SubmitReturnVersion;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SubmitReturnVersionDataAccessTests
    {
        [Fact]
        public async Task Submit_SetsDataReturnVersionAsSubmitted()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = helper.CreateScheme();
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, false);

                database.Model.SaveChanges();

                var dbDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);
                var dataAccess = new SubmitReturnVersionDataAccess(database.WeeeContext);

                // Act
                await dataAccess.Submit(dbDataReturnVersion);

                // Assert
                Assert.Equal(dataReturnVersion.Id, dbDataReturnVersion.Id);
                Assert.True(dbDataReturnVersion.IsSubmitted);
                Assert.NotNull(dbDataReturnVersion.SubmittedDate);
                Assert.Equal(database.WeeeContext.GetCurrentUser(), dbDataReturnVersion.SubmittingUserId);
                Assert.Equal(dataReturnVersion.Id, dbDataReturnVersion.DataReturn.CurrentVersion.Id);
            }
        }
    }
}
