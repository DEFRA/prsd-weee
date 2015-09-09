namespace EA.Weee.Core.Tests.Unit.Routing
{
    using EA.Weee.Core.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ResetPasswordRouteTests
    {
        [Fact]
        public void GenerateUrl_WithUrlSafeValues_ReplacesPlaceholders()
        {
            // Arrange
            string url = "https://domain.com/" + ResetPasswordRoute.PlaceholderUserId + "/" + ResetPasswordRoute.PlaceholderToken + "/foo";

            string userId = "user7";
            string token = "token9";

            ResetPasswordRoute route = new ResetPasswordRoute(url);
            route.UserID = userId;
            route.Token = token;

            // Act
            string result = route.GenerateUrl();

            // Assert
            Assert.Equal("https://domain.com/user7/token9/foo", result);
        }

        [Fact]
        public void GenerateUrl_WithUrlUnsafeValues_ReplacesPlaceholdersWithUrlEncoding()
        {
            // Arrange
            string url = "https://domain.com/" + ResetPasswordRoute.PlaceholderUserId + "/" + ResetPasswordRoute.PlaceholderToken + "/foo";

            string userId = "user/7";
            string token = "token/9";

            ResetPasswordRoute route = new ResetPasswordRoute(url);
            route.UserID = userId;
            route.Token = token;

            // Act
            string result = route.GenerateUrl();

            // Assert
            Assert.Equal("https://domain.com/user%2f7/token%2f9/foo", result);
        }
    }
}
