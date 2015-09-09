namespace EA.Weee.Core.Tests.Unit.Routing
{
    using EA.Weee.Core.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ExternalRouteTests
    {
        [Fact]
        public void GenerateUrl_WithValidUrl_ReturnsUrlUnchanged()
        {
            // Arrange
            string url = "https://domain.com/somecontrollers/someaction?q=foo";

            ExternalRoute externalRoute = new ExternalRoute(url);

            // Act
            string result = externalRoute.GenerateUrl();

            // Assert
            Assert.Equal(url, result);
        }
    }
}
