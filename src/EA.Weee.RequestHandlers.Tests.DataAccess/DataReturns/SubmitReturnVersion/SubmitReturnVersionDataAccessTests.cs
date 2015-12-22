namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.SubmitReturnVersion
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using RequestHandlers.DataReturns.SubmitReturnVersion;
    using Weee.DataAccess;
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

                var scheme = helper.CreateScheme();
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, false);

                database.Model.SaveChanges();

                var dbDataReturnVersion = GetDomainDataReturnVersion(database.WeeeContext, dataReturnVersion.Id);

                var dataAccess = new SubmitReturnVersionDataAccess(database.WeeeContext);
                await dataAccess.Submit(dbDataReturnVersion);

                Assert.Equal(dataReturnVersion.Id, dbDataReturnVersion.Id);
                Assert.True(dbDataReturnVersion.IsSubmitted);
                Assert.NotNull(dbDataReturnVersion.SubmittedDate);
                Assert.Equal(dataReturnVersion.Id, dbDataReturnVersion.DataReturn.CurrentVersion.Id);
            }
        }

        private Domain.DataReturns.DataReturnVersion GetDomainDataReturnVersion(WeeeContext context, Guid id)
        {
            return context.DataReturnVersions.Single(r => r.Id == id);
        }
    }
}
