namespace EA.Prsd.Core.Tests.Web
{
    using System;
    using System.Globalization;
    using System.Security.Claims;
    using Core.Web.Extensions;
    using IdentityModel;
    using IdentityModel.Client;
    using Xunit;

    public class TokenResponseExtensionsTests : IDisposable
    {
        private readonly ClaimsIdentity identity;

        public TokenResponseExtensionsTests()
        {
            // Freeze time at 2015/01/01 00:00:00
            SystemTime.Freeze(new DateTime(2015, 1, 1, 0, 0, 0));

            var response = new TokenResponse("{access_token:123,refresh_token:456,expires_in:3600}");
            identity = response.GenerateUserIdentity("TEST");
        }

        public void Dispose()
        {
            SystemTime.Unfreeze();
        }

        [Fact]
        public void AccessTokenClaimIsSet()
        {
            Assert.Equal("123", identity.FindFirst(OidcConstants.AuthorizeResponse.AccessToken).Value);
        }

        [Fact]
        public void RefreshTokenClaimIsSet()
        {
            Assert.Equal("456", identity.FindFirst(OidcConstants.AuthorizeResponse.RefreshToken).Value);
        }

        [Fact]
        public void ExpiresAtClaimIsSet()
        {
            Assert.Equal("2015-01-01 01:00:00Z", identity.FindFirst(Core.Web.ClaimTypes.ExpiresAt).Value);
        }

        [Fact]
        public void CanParseExpiresAt()
        {
            var expected = new DateTime(2015, 1, 1, 1, 0, 0, DateTimeKind.Utc);
            var actual = DateTime.ParseExact(identity.FindFirst(Core.Web.ClaimTypes.ExpiresAt).Value, "u", CultureInfo.InvariantCulture);

            Assert.Equal(expected, actual);
        }
    }
}