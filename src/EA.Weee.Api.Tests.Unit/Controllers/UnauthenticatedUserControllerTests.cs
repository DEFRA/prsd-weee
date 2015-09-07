﻿namespace EA.Weee.Api.Tests.Unit.Controllers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client.Entities;
    using EA.Weee.Api.Controllers;
    using EA.Weee.Api.Identity;
    using EA.Weee.Core;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Email;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Xunit;

    public class UnauthenticatedUserControllerTests
    {
        [Fact]
        public async Task CreateInternalUser_WithValidModel_IssuesCanAccessInternalAreaClaim()
        {
            // Arrange
            ApplicationUser capturedUser = null;
            ApplicationUserManager userManager = A.Fake<ApplicationUserManager>();

            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._))
                .Invokes((ApplicationUser user) => capturedUser = user)
                .Returns(IdentityResult.Success);

            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._, A<string>._))
                .Invokes((ApplicationUser user, string password) => capturedUser = user)
                .Returns(IdentityResult.Success);

            IUserContext userContext = A.Fake<IUserContext>();
            IWeeeEmailService emailService = A.Fake<IWeeeEmailService>();

            UnauthenticatedUserController controller = new UnauthenticatedUserController(
                userManager,
                userContext,
                emailService);

            InternalUserCreationData model = new InternalUserCreationData()
            {
                FirstName = "John",
                Surname = "Smith",
                Email = "john.smith@environment-agency.gov.uk",
                Password = "Password1",
                ConfirmPassword = "Password1",
                ActivationBaseUrl = "ActivationBaseUrl"
            };

            // Act
            await controller.CreateInternalUser(model);

            // Assert
            Assert.NotNull(capturedUser);

            int claimCount = capturedUser.Claims.Count(
                c => c.ClaimType == ClaimTypes.AuthenticationMethod
                && c.ClaimValue == Claims.CanAccessInternalArea);

            Assert.True(claimCount == 1, "A single \"CanAccessInternalArea\" claim was not issued to the user.");
        }

        [Fact]
        public async Task CreateExternalUser_WithValidModel_IssuesCanAccessExternalAreaClaim()
        {
            // Arrange
            ApplicationUser capturedUser = null;
            ApplicationUserManager userManager = A.Fake<ApplicationUserManager>();

            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._))
                .Invokes((ApplicationUser user) => capturedUser = user)
                .Returns(IdentityResult.Success);

            A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._, A<string>._))
                .Invokes((ApplicationUser user, string password) => capturedUser = user)
                .Returns(IdentityResult.Success);

            IUserContext userContext = A.Fake<IUserContext>();
            IWeeeEmailService emailService = A.Fake<IWeeeEmailService>();

            UnauthenticatedUserController controller = new UnauthenticatedUserController(
                userManager,
                userContext,
                emailService);

            ExternalUserCreationData model = new ExternalUserCreationData()
            {
                FirstName = "John",
                Surname = "Smith",
                Email = "john.smith@domain.com",
                Password = "Password1",
                ConfirmPassword = "Password1",
                ActivationBaseUrl = "ActivationBaseUrl"
            };

            // Act
            await controller.CreateExternalUser(model);

            // Assert
            Assert.NotNull(capturedUser);

            int claimCount = capturedUser.Claims.Count(
                c => c.ClaimType == ClaimTypes.AuthenticationMethod
                && c.ClaimValue == Claims.CanAccessExternalArea);

            Assert.True(claimCount == 1, "A single \"CanAccessExterna;Area\" claim was not issued to the user.");
        }
    }
}
