namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Tests.Core.Model;
    using System.Threading.Tasks;
    using Xunit;

    public class GetMessageBannerDataAccessTests
    {
        [Fact]
        public async Task GetMessageBanner_WithValid_Data()
        {
            using (var database = new DatabaseWrapper())
            {
                GetMessageBannerDataAccess dataAccess = new GetMessageBannerDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var messageBanner = modelHelper.CreateMessageBanner();
                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetMessageBannerData();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.ID);
                Assert.Equal("Test Title", result.Title);
                Assert.Equal("Test Description", result.Description);
            }
        }

        [Fact]
        public async Task GetMessageBanner_WithNoValid_Data()
        {
            using (var database = new DatabaseWrapper())
            {
                GetMessageBannerDataAccess dataAccess = new GetMessageBannerDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var messageBanner = modelHelper.CreateOldMessageBanner();
                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetMessageBannerData();

                // Assert
                Assert.Null(result);
            }
        }
    }
}
