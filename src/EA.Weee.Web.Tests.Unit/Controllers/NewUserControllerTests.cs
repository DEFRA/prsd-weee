namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Api.Client;
    using Api.Client.Actions;
    using Api.Client.Entities;
    using Core;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using ViewModels.NewUser;
    using Web.Controllers;
    using Xunit;

    public class NewUserControllerTests
    {
        private readonly IOAuthClient oathClient;
        private readonly IWeeeClient weeeClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IExternalRouteService externalRouteService;

        public NewUserControllerTests()
        {
            oathClient = A.Fake<IOAuthClient>();
            weeeClient = A.Fake<IWeeeClient>();
            authenticationManager = A.Fake<IAuthenticationManager>();
            externalRouteService = A.Fake<IExternalRouteService>();
        }

        [Fact]
        public async Task NewUser_NameNotProvided_ValidationError()
        {
            var userCreationViewModel = GetValidUserCreationViewModel();

            userCreationViewModel.Name = string.Empty;

            var newUserController = GetMockNewUserController(userCreationViewModel);

            var result = await newUserController.UserCreation(userCreationViewModel) as ViewResult;

            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task NewUser_SurnameNotProvided_ValidationError()
        {
            var userCreationViewModel = GetValidUserCreationViewModel();

            userCreationViewModel.Surname = string.Empty;

            var newUserController = GetMockNewUserController(userCreationViewModel);

            var result = await newUserController.UserCreation(userCreationViewModel) as ViewResult;

            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        [InlineData("test@")]
        [InlineData("test.com")]
        public async Task NewUser_EmailInvalidOrNotProvided_ValidationError(string expected)
        {
            var userCreationViewModel = GetValidUserCreationViewModel();

            userCreationViewModel.Email = expected;

            var newUserController = GetMockNewUserController(userCreationViewModel);

            var result = await newUserController.UserCreation(userCreationViewModel) as ViewResult;

            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task NewUser_TermsAndConditionsNotChecked_ValidationError()
        {
            var userCreationViewModel = GetValidUserCreationViewModel();

            userCreationViewModel.TermsAndConditions = false;

            var newUserController = GetMockNewUserController(userCreationViewModel);

            var result = await newUserController.UserCreation(userCreationViewModel) as ViewResult;

            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task HttpPost_NewUser_IsValid()
        {
            var userCreationViewModel = GetValidUserCreationViewModel();
            var newUser = A.Fake<IUnauthenticatedUser>();

            var userCreationData = new InternalUserCreationData();
            A.CallTo(() => newUser.CreateInternalUserAsync(A<InternalUserCreationData>._))
                .Invokes((InternalUserCreationData u) => userCreationData = u)
                .Returns(Task.FromResult(A<string>._));

            A.CallTo(() => weeeClient.User).Returns(newUser);

            await NewUserController().UserCreation(userCreationViewModel);
        }

        private NewUserController NewUserController()
        {
            return new NewUserController(
                () => oathClient,
                () => weeeClient,
                authenticationManager,
                externalRouteService);
        }

        private NewUserController GetMockNewUserController(object viewModel)
        {
            var newuserController = new NewUserController(
                () => new OAuthClient("test", "test", "test"),
                () => new WeeeClient("test"),
                null,
                externalRouteService);
            
            // Mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(viewModel, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModel, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                newuserController.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }

            return newuserController;
        }

        private static UserCreationViewModel GetValidUserCreationViewModel()
        {
            const string validEmail = "test@test.com";
            const string validPassword = "P@ssword1";
            const string validName = "ValidName";
            const string validSurname = "ValidSurname";

            var validuserCreationViewModel = new UserCreationViewModel
            {
                Email = validEmail,
                Password = validPassword,
                ConfirmPassword = validPassword,
                Name = validName,
                Surname = validSurname,
                TermsAndConditions = true
            };

            return validuserCreationViewModel;
        }
    }
}
