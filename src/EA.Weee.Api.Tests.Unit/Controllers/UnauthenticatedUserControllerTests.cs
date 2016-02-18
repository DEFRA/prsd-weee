namespace EA.Weee.Api.Tests.Unit.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
    using Core.Routing;
    using Domain;
    using Domain.Admin;
    using Domain.User;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Api.Client.Entities;
    using EA.Weee.Api.Controllers;
    using EA.Weee.Api.Identity;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Email;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using RequestHandlers.Admin;
    using Security;
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
            IGetAdminUserDataAccess getAdminUserDataAccess = A.Dummy<IGetAdminUserDataAccess>();

            UnauthenticatedUserController controller = new UnauthenticatedUserController(
                userManager,
                userContext,
                emailService,
                getAdminUserDataAccess);

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
            IGetAdminUserDataAccess getAdminUserDataAccess = A.Dummy<IGetAdminUserDataAccess>();

            UnauthenticatedUserController controller = new UnauthenticatedUserController(
                userManager,
                userContext,
                emailService,
                getAdminUserDataAccess);

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

        [Fact]
        public async Task ActivateUserAccount_DoesNotSendCompetentAuthorityEmail_WhenEmailConfirmationFails()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Failed());

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(A.Dummy<ActivatedUserAccountData>());

            // Assert
            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(A<Guid>._))
                .MustNotHaveHappened();
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task ActivateUserAccount_DoesNotSendCompetentAuthorityEmail_WhenUserIsNotInternalUser()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(A<Guid>._))
                .Returns((CompetentAuthorityUser)null);

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(A.Dummy<ActivatedUserAccountData>());

            // Assert
            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(A<Guid>._))
                .MustHaveHappened();
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task ActivateUserAccount_SendsCompetentAuthorityEmail_WhenUserStatusIsPendingOnly(int userStatus)
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userId = Guid.NewGuid();
            string emailAddress = "test@sfwltd.co.uk";

            var competentAuthority = new UKCompetentAuthority(
                A.Dummy<Guid>(),
                "EA",
                "EA",
                A.Dummy<Country>(),
                emailAddress);

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            A.CallTo(() => competentAuthorityUser.CompetentAuthority)
                .Returns(competentAuthority);
            A.CallTo(() => competentAuthorityUser.UserStatus)
                .Returns(Enumeration.FromValue<UserStatus>(userStatus));

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(userId))
                .Returns(competentAuthorityUser);

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            var activatedUserAccountData = new ActivatedUserAccountData
            {
                Id = userId,
                ViewUserRoute = viewUserRoute
            };

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(activatedUserAccountData);

            // Assert
            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(A<Guid>._))
                .MustHaveHappened();
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task ActivateUserAccount_SendsEmailToCompetentAuthorityEmailAddress()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userId = Guid.NewGuid();
            string emailAddress = "test@sfwltd.co.uk";

            var competentAuthority = new UKCompetentAuthority(
                A.Dummy<Guid>(),
                "EA",
                "EA",
                A.Dummy<Country>(),
                emailAddress);

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            A.CallTo(() => competentAuthorityUser.CompetentAuthority)
                .Returns(competentAuthority);
            A.CallTo(() => competentAuthorityUser.UserStatus)
                .Returns(UserStatus.Pending);

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(userId))
                .Returns(competentAuthorityUser);

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            var activatedUserAccountData = new ActivatedUserAccountData
            {
                Id = userId,
                ViewUserRoute = viewUserRoute
            };

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(activatedUserAccountData);

            // Assert
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(emailAddress, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task ActivateUserAccount_SendsEmailWithFullNameOfUser()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userId = Guid.NewGuid();
            var user = new User(userId.ToString(), "FirstName", "SecondName", "test@sfwltd.co.uk");

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            A.CallTo(() => competentAuthorityUser.User)
                .Returns(user);
            A.CallTo(() => competentAuthorityUser.UserStatus)
                .Returns(UserStatus.Pending);

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(userId))
                .Returns(competentAuthorityUser);

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            var activatedUserAccountData = new ActivatedUserAccountData
            {
                Id = userId,
                ViewUserRoute = viewUserRoute
            };

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(activatedUserAccountData);

            // Assert
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, "FirstName SecondName", A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task ActivateUserAccount_SendsEmailWithEmailAddressOfUser()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userId = Guid.NewGuid();
            var user = new User(userId.ToString(), "FirstName", "SecondName", "test@sfwltd.co.uk");

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            A.CallTo(() => competentAuthorityUser.User)
                .Returns(user);
            A.CallTo(() => competentAuthorityUser.UserStatus)
                .Returns(UserStatus.Pending);

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(userId))
                .Returns(competentAuthorityUser);

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            var activatedUserAccountData = new ActivatedUserAccountData
            {
                Id = userId,
                ViewUserRoute = viewUserRoute
            };

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(activatedUserAccountData);

            // Assert
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, A<string>._, "test@sfwltd.co.uk", A<string>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task ActivateUserAccount_SendsEmailWithViewUserUrl()
        {
            // Arrange
            var builder = new UnauthenticatedUserControllerBuilder();

            A.CallTo(() => builder.UserManager.ConfirmEmailAsync(A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userId = Guid.NewGuid();
            var user = new User(userId.ToString(), "FirstName", "SecondName", "test@sfwltd.co.uk");

            var competentAuthorityUser = A.Fake<CompetentAuthorityUser>();
            A.CallTo(() => competentAuthorityUser.User)
                .Returns(user);
            A.CallTo(() => competentAuthorityUser.UserStatus)
                .Returns(UserStatus.Pending);

            A.CallTo(() => builder.GetAdminUserDataAccess.GetAdminUserOrDefault(userId))
                .Returns(competentAuthorityUser);

            string viewUserUrl = "http://localhost/EditUser/abc";

            var viewUserRoute = A.Fake<ViewCompetentAuthorityUserRoute>();
            A.CallTo(() => viewUserRoute.GenerateUrl())
                .Returns(viewUserUrl);

            var activatedUserAccountData = new ActivatedUserAccountData
            {
                Id = userId,
                ViewUserRoute = viewUserRoute
            };

            var controller = builder.Build();

            // Act
            await controller.ActivateUserAccount(activatedUserAccountData);

            // Assert
            A.CallTo(() => builder.EmailService.SendInternalUserAccountActivated(A<string>._, A<string>._, A<string>._, viewUserUrl))
                .MustHaveHappened();
        }

        private class UnauthenticatedUserControllerBuilder
        {
            public ApplicationUserManager UserManager { get; private set; }
            public IUserContext UserContext { get; private set; }
            public IWeeeEmailService EmailService { get; private set; }
            public IGetAdminUserDataAccess GetAdminUserDataAccess { get; private set; }

            public UnauthenticatedUserControllerBuilder()
            {
                UserManager = A.Fake<ApplicationUserManager>();
                UserContext = A.Fake<IUserContext>();
                EmailService = A.Fake<IWeeeEmailService>();
                GetAdminUserDataAccess = A.Fake<IGetAdminUserDataAccess>();
            }

            public UnauthenticatedUserController Build()
            {
                return new UnauthenticatedUserController(UserManager, UserContext, EmailService, GetAdminUserDataAccess);
            }
        }
    }
}
