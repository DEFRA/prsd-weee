namespace EA.Weee.Api.Tests.Unit.Controllers
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
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
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

        [Fact]
        public async Task ResetPasswordRequest_InvalidEmailAddress_DoesNotReturnToken()
        {
            var builder = new UnauthenticatedUserControllerBuilder();
            var controller = builder.Build();
            var userManager = builder.UserManager;

            A.CallTo(() => userManager.FindByEmailAsync(A<string>._)).Returns((ApplicationUser)null);

            var result = (OkNegotiatedContentResult<PasswordResetRequestResult>)await controller.ResetPasswordRequest(A.Fake<PasswordResetRequest>());
            var passwordResetRequestResult = result.Content;

            A.CallTo(() => userManager.FindByEmailAsync(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => userManager.GeneratePasswordResetTokenAsync(A<string>._)).MustNotHaveHappened();
            Assert.False(passwordResetRequestResult.ValidEmail);
            Assert.Null(passwordResetRequestResult.PasswordResetToken);
        }

        [Fact]
        public async Task ResetPasswordRequest_ValidEmailAddress_ReturnsToken()
        {
            var builder = new UnauthenticatedUserControllerBuilder();
            var controller = builder.Build();
            var userManager = builder.UserManager;

            string resetToken = "Test token";

            A.CallTo(() => userManager.FindByEmailAsync(A<string>._)).Returns(A.Fake<ApplicationUser>());
            A.CallTo(() => userManager.GeneratePasswordResetTokenAsync(A<string>._)).Returns(resetToken);

            var result = (OkNegotiatedContentResult<PasswordResetRequestResult>)await controller.ResetPasswordRequest(A.Fake<PasswordResetRequest>());
            var passwordResetRequestResult = result.Content;

            Assert.True(passwordResetRequestResult.ValidEmail);
            Assert.Equal(resetToken, passwordResetRequestResult.PasswordResetToken);
        }

        private class UnauthenticatedUserControllerBuilder
        {
            public ApplicationUserManager UserManager { get; set; }
            public IUserContext UserContext { get; set; }
            public IWeeeEmailService EmailService { get; set; }

            public UnauthenticatedUserControllerBuilder()
            {
                UserManager = A.Fake<ApplicationUserManager>();
                UserContext = A.Fake<IUserContext>();
                EmailService = A.Fake<IWeeeEmailService>();
            }

            public UnauthenticatedUserController Build()
            {
                return new UnauthenticatedUserController(UserManager, UserContext, EmailService);
            }
        }
    }
}
