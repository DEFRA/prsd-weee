namespace EA.Weee.Core.Tests.Unit.Routing
{
    using Core.Routing;
    using Xunit;

    public class ViewCompetentAuthorityUserRouteTests
    {
        [Fact]
        public void GenerateUrl_WithUrlSafeValues_ReplacesPlaceholders()
        {
            // Arrange
            string url = "https://domain.com/" + ViewCompetentAuthorityUserRoute.PlaceholderUserId;

            string userId = "user7";

            ViewCompetentAuthorityUserRoute route = new ViewCompetentAuthorityUserRoute(url);
            route.CompetentAuthorityUserID = userId;

            // Act
            string result = route.GenerateUrl();

            // Assert
            Assert.Equal("https://domain.com/user7", result);
        }

        [Fact]
        public void GenerateUrl_WithUrlUnsafeValues_ReplacesPlaceholdersWithUrlEncoding()
        {
            // Arrange
            string url = "https://domain.com/" + ViewCompetentAuthorityUserRoute.PlaceholderUserId;

            string userId = "user/7";

            ViewCompetentAuthorityUserRoute route = new ViewCompetentAuthorityUserRoute(url);
            route.CompetentAuthorityUserID = userId;

            // Act
            string result = route.GenerateUrl();

            // Assert
            Assert.Equal("https://domain.com/user%2f7", result);
        }
    }
}
