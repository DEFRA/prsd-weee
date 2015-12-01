namespace EA.Weee.Web.Tests.Unit.App_Start
{
    using EA.Weee.Web.App_Start;
    using FakeItEasy;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Xunit;

    public class WeeeCookieAuthenticationProviderTests
    {
        /// <summary>
        /// This test ensures that the redirect URI is unchanged for a value of ReturnUrl
        /// that is not mapped.
        /// </summary>
        [Fact]
        public void ApplyReturnUrlMapping_ReturnUrlNotMapped_ReturnsRedirectUriUnchanged()
        {
            // Arrange
            CookieApplyRedirectContext context = A.Fake<CookieApplyRedirectContext>();
            context.RedirectUri = "https://weee.com/sign-in?ReturnUrl=%2fcontroller1%2faction1";

            IReturnUrlMapping mapping = A.Fake<IReturnUrlMapping>();
            A.CallTo(() => mapping.IsMapped("/mycontroller/myaction")).Returns(false);

            WeeeCookieAuthenticationProvider provider = new WeeeCookieAuthenticationProvider(mapping);

            // Act
            provider.ApplyReturnUrlMapping(context);

            // Assert
            Assert.Equal("https://weee.com/sign-in?ReturnUrl=%2fcontroller1%2faction1", context.RedirectUri);
        }

        /// <summary>
        /// This test ensures that the redirect URI is updated for a value of ReturnUrl
        /// that is mapped to a new return URL.
        /// </summary>
        [Fact]
        public void ApplyReturnUrlMapping_ReturnUrlMappedToNewReturnUrl_ReturnsRedirectUriWithNewReturnUrl()
        {
            // Arrange
            CookieApplyRedirectContext context = A.Fake<CookieApplyRedirectContext>();
            context.RedirectUri = "https://weee.com/sign-in?ReturnUrl=%2fcontroller1%2faction1";

            IReturnUrlMapping mapping = A.Fake<IReturnUrlMapping>();
            A.CallTo(() => mapping.IsMapped("/controller1/action1")).Returns(true);
            A.CallTo(() => mapping.ApplyMap("/controller1/action1")).Returns("/controller2/action2");

            WeeeCookieAuthenticationProvider provider = new WeeeCookieAuthenticationProvider(mapping);

            // Act
            provider.ApplyReturnUrlMapping(context);

            // Assert
            Assert.Equal("https://weee.com/sign-in?ReturnUrl=%2fcontroller2%2faction2", context.RedirectUri);
        }

        /// <summary>
        /// This test ensures that the return URL is removed from the redirect URI
        /// for a value of ReturnUrl that is mapped to null.
        /// </summary>
        [Fact]
        public void ApplyReturnUrlMapping_ReturnUrlMappedToNull_ReturnsRedirectUriWithoutReturnUrl()
        {
            // Arrange
            CookieApplyRedirectContext context = A.Fake<CookieApplyRedirectContext>();
            context.RedirectUri = "https://weee.com/sign-in?ReturnUrl=%2fcontroller1%2faction1";

            IReturnUrlMapping mapping = A.Fake<IReturnUrlMapping>();
            A.CallTo(() => mapping.IsMapped("/controller1/action1")).Returns(true);
            A.CallTo(() => mapping.ApplyMap("/controller1/action1")).Returns(null);

            WeeeCookieAuthenticationProvider provider = new WeeeCookieAuthenticationProvider(mapping);

            // Act
            provider.ApplyReturnUrlMapping(context);

            // Assert
            Assert.Equal("https://weee.com/sign-in", context.RedirectUri);
        }
    }
}
