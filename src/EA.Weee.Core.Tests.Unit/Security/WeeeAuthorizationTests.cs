namespace EA.Weee.Core.Tests.Unit.Security
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.Security;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class WeeeAuthorizationTests
    {
        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessInternalArea_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);
            
            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            Action action = () => authorization.EnsureCanAccessInternalArea();

            // Assert
            Assert.Throws(typeof(SecurityException), action);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ReturnsTrue_WhenUserHasRequiredClaim()
        {
            // Arrange
            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));
            
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea();

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessExternalArea_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            Action action = () => authorization.EnsureCanAccessExternalArea();

            // Assert
            Assert.Throws(typeof(SecurityException), action);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessExternalArea_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckCanAccessExternalArea();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessExternalArea_ReturnsTrue_WhenUserHasRequiredClaim()
        {
            // Arrange
            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessExternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckCanAccessExternalArea();

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureOrganisationAccess_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            Action action = () => authorization.EnsureOrganisationAccess(organisationID);

            // Assert
            Assert.Throws(typeof(SecurityException), action);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckOrganisationAccess_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckOrganisationAccess_ReturnsTrue_WhenUserHasRequiredClaims()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(WeeeClaimTypes.OrganisationAccess, organisationID.ToString()));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureSchemeAccess_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            Guid schemeID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            Action action = () => authorization.EnsureSchemeAccess(schemeID);

            // Assert
            Assert.Throws(typeof(SecurityException), action);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckSchemeAccess_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            Guid schemeID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckSchemeAccess(schemeID);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckSchemeAccess_ReturnsTrue_WhenUserHasRequiredClaims()
        {
            // Arrange
            Guid schemeID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(WeeeClaimTypes.SchemeAccess, schemeID.ToString()));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(userContext);

            // Act
            bool result = authorization.CheckSchemeAccess(schemeID);

            // Assert
            Assert.Equal(true, result);
        }
    }
}
