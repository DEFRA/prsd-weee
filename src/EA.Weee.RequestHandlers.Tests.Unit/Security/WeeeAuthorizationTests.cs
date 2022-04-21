﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Security
{
    using DataAccess;
    using Domain.Admin;
    using Domain.Organisation;
    using Domain.User;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Security;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Security.Claims;
    using Domain.AatfReturn;
    using FluentAssertions;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;
    using Role = Domain.Security.Role;

    public class WeeeAuthorizationTests
    {
        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessInternalArea_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            Action action = () => authorization.EnsureCanAccessInternalArea();

            // Assert
            Assert.Throws(typeof(SecurityException), action);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustNotHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessInternalArea_ThrowsSecurityException_WhenUserHasClaimsAndIsNotActive()
        {
            // Arrange
            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userStatusActive: false);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            Action action = () => authorization.EnsureCanAccessInternalArea();

            // Assert
            Assert.Throws(typeof(SecurityException), action);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessInternalArea_ActiveUserNotRequired_DoesNotThrowSecurityException_WhenUserHasClaimsAndIsNotActive()
        {
            // Arrange
            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userStatusActive: false);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            var ex = Record.Exception(() => authorization.EnsureCanAccessInternalArea(false));

            // Assert
            Assert.Null(ex);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustNotHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea();

            // Assert
            Assert.Equal(false, result);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustNotHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ReturnsFalse_WhenUserHasClaimsAndIsNotActive()
        {
            // Arrange
            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userStatusActive: false);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea();

            // Assert
            Assert.Equal(false, result);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ReturnsTrue_WhenUserHasRequiredClaimAndIsActive()
        {
            // Arrange
            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea();

            // Assert
            Assert.Equal(true, result);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckCanAccessInternalArea_ActiveUserNotRequired_ReturnsTrue_WhenUserHasClaimsAndIsNotActive()
        {
            // Arrange
            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckCanAccessInternalArea(false);

            // Assert
            Assert.Equal(true, result);
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).MustNotHaveHappened();
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureCanAccessExternalArea_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            ClaimsPrincipal principal = new ClaimsPrincipal();
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessExternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            IUserContext userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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
            Guid userId = Guid.NewGuid();

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext =
                MakeFakeWeeeContext(
                    userContext,
                    userId,
                    new List<OrganisationUser> { new OrganisationUser(userId, organisationID, UserStatus.Active) });

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureInternalOrOrganisationAccess_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            Action action = () => authorization.EnsureInternalOrOrganisationAccess(organisationID);

            // Assert
            Assert.Throws(typeof(SecurityException), action);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckInternalOrOrganisationAccess_ReturnsFalse_WhenUserHasNoClaims()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckInternalOrOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckInternalOrOrganisationAccess_ReturnsTrue_WhenUserHasRequiredOrganisationClaim()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");
            Guid userId = Guid.NewGuid();

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext =
                MakeFakeWeeeContext(
                    userContext,
                    userId,
                    new List<OrganisationUser> { new OrganisationUser(userId, organisationID, UserStatus.Active) });

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckInternalOrOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckInternalOrOrganisationAccess_ReturnsTrue_WhenUserHasRequiredInternalClaim()
        {
            // Arrange
            Guid organisationID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");
            Guid userId = Guid.NewGuid();

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext =
                MakeFakeWeeeContext(
                    userContext,
                    userId,
                    new List<OrganisationUser> { new OrganisationUser(userId, organisationID, UserStatus.Active) });

            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            A.CallTo(() => userContext.Principal).Returns(principal);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckInternalOrOrganisationAccess(organisationID);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void EnsureSchemeAccess_ThrowsSecurityException_WhenUserHasNoClaims()
        {
            // Arrange
            Guid schemeID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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

            IUserContext userContext = A.Fake<IUserContext>();
            WeeeContext weeeContext = MakeFakeWeeeContext(userContext);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

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
            Guid organisationID = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid schemeID = new Guid("5F3069F4-EDA3-43A3-BDD8-726028CDABB0");
            Domain.Scheme.Scheme scheme = new Domain.Scheme.Scheme(organisationID);
            typeof(Entity).GetProperty("Id").SetValue(scheme, schemeID); // <- sad but necessary

            IUserContext userContext = A.Fake<IUserContext>();

            WeeeContext weeeContext =
                MakeFakeWeeeContext(
                    userContext,
                    userId,
                    new List<OrganisationUser> { new OrganisationUser(userId, organisationID, UserStatus.Active) },
                    new List<Domain.Scheme.Scheme> { scheme });

            A.CallTo(() => userContext.UserId).Returns(userId);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckSchemeAccess(schemeID);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckUserInRole_ReturnsFalse_WhenUserIsNotInRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IUserContext userContext = A.Fake<IUserContext>();

            var competentAuthorityUsers = new List<CompetentAuthorityUser>
            {
                new CompetentAuthorityUser(userId.ToString(), Guid.NewGuid(), UserStatus.Active, new Role("InternalUser", "Standard user"))
            };

            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userId: userId, competentAuthorityUsers: competentAuthorityUsers);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckUserInRole(Roles.InternalAdmin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Area", "Security")]
        public void CheckUserInRole_ReturnsTrue_WhenUserIsInRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IUserContext userContext = A.Fake<IUserContext>();

            var competentAuthorityUsers = new List<CompetentAuthorityUser>
            {
                new CompetentAuthorityUser(userId.ToString(), Guid.NewGuid(), UserStatus.Active, new Role("InternalAdmin", "Administrator"))
            };

            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userId: userId, competentAuthorityUsers: competentAuthorityUsers);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act
            bool result = authorization.CheckUserInRole(Roles.InternalAdmin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EnsureUserInRole_ThrowsSecurityException_WhenUserIsNotInRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IUserContext userContext = A.Fake<IUserContext>();

            var competentAuthorityUsers = new List<CompetentAuthorityUser>
            {
                new CompetentAuthorityUser(userId.ToString(), Guid.NewGuid(), UserStatus.Active, new Role("InternalUser", "Standard user"))
            };

            WeeeContext weeeContext = MakeFakeWeeeContext(userContext, userId: userId, competentAuthorityUsers: competentAuthorityUsers);

            WeeeAuthorization authorization = new WeeeAuthorization(weeeContext, userContext);

            // Act, Assert
            Assert.Throws<SecurityException>(() => authorization.EnsureUserInRole(Roles.InternalAdmin));
        }

        [Fact]
        public void EnsureAatfHasOrganisationAccess_GivenAatfIsNotFound_ArgumentNullExceptionExpected()
        {
            //arrange
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();

            var weeeContext = MakeFakeWeeeContext(userContext, userId, aatfs: new List<Aatf>());
            var authorization = new WeeeAuthorization(weeeContext, userContext);

            //act
            var exception = Record.Exception(() => authorization.EnsureAatfHasOrganisationAccess(Guid.NewGuid()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void EnsureAatfHasOrganisationAccess_GivenAatfIsNotAssociatedWithLoggedInOrganisation_SecurityExceptionExpected()
        {
            //arrange
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);

            var weeeContext = MakeFakeWeeeContext(userContext, userId, aatfs: new List<Aatf>()
            {
                aatf
            });

            var authorization = new WeeeAuthorization(weeeContext, userContext);

            //act
            var exception = Record.Exception(() => authorization.EnsureAatfHasOrganisationAccess(aatfId));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public void EnsureAatfHasOrganisationAccess_GivenAatfIsAssociatedWithLoggedInOrganisation_ExceptionShouldNotBeThrown()
        {
            //arrange
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.OrganisationId).Returns(organisationId);

            var weeeContext = MakeFakeWeeeContext(userContext, userId, aatfs: new List<Aatf>()
            {
                aatf
            }, organisationUsers: new List<OrganisationUser> { new OrganisationUser(userId, organisationId, UserStatus.Active) });

            var authorization = new WeeeAuthorization(weeeContext, userContext);

            //act
            var exception = Record.Exception(() => authorization.EnsureAatfHasOrganisationAccess(aatfId));

            //assert
            exception.Should().BeNull();
        }

        [Fact]
        public void EnsureAatfHasOrganisationAccess_GivenAatfIsAssociatedWithLoggedInOrganisationButUserIsInactive_ExceptionShouldBeThrown()
        {
            //assert
            ShouldThrowSecurityExceptionForAatfWhenUserIsInactive(UserStatus.Inactive);
            ShouldThrowSecurityExceptionForAatfWhenUserIsInactive(UserStatus.Rejected);
            ShouldThrowSecurityExceptionForAatfWhenUserIsInactive(UserStatus.Pending);
        }

        private void ShouldThrowSecurityExceptionForAatfWhenUserIsInactive(UserStatus status)
        {
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.OrganisationId).Returns(organisationId);

            var weeeContext = MakeFakeWeeeContext(userContext, userId, aatfs: new List<Aatf>() { aatf },
                organisationUsers: new List<OrganisationUser> { new OrganisationUser(userId, organisationId, status) });

            var authorization = new WeeeAuthorization(weeeContext, userContext);

            //act
            var exception = Record.Exception(() => authorization.EnsureAatfHasOrganisationAccess(aatfId));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        private WeeeContext MakeFakeWeeeContext(IUserContext userContext,
                                                Guid? userId = null,
                                                List<OrganisationUser> organisationUsers = null,
                                                List<Domain.Scheme.Scheme> schemes = null,
                                                bool userStatusActive = true,
                                                List<CompetentAuthorityUser> competentAuthorityUsers = null,
                                                List<Aatf> aatfs = null)
        {
            userId = userId ?? Guid.NewGuid();

            organisationUsers = organisationUsers ?? new List<OrganisationUser>();
            schemes = schemes ?? new List<Domain.Scheme.Scheme>();
            competentAuthorityUsers = competentAuthorityUsers ?? new List<CompetentAuthorityUser>
            {
                new CompetentAuthorityUser(userId.ToString(), Guid.NewGuid(), userStatusActive ? UserStatus.Active : UserStatus.Inactive, A.Dummy<Role>())
            };

            var dbHelper = new DbContextHelper();
            WeeeContext weeeContext = A.Fake<WeeeContext>();
            A.CallTo(() => weeeContext.OrganisationUsers).Returns(dbHelper.GetAsyncEnabledDbSet(organisationUsers));
            A.CallTo(() => weeeContext.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(schemes));
            A.CallTo(() => weeeContext.CompetentAuthorityUsers).Returns(dbHelper.GetAsyncEnabledDbSet(competentAuthorityUsers));
            A.CallTo(() => weeeContext.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(aatfs));

            A.CallTo(() => userContext.UserId).Returns(userId.Value);

            return weeeContext;
        }
    }
}
