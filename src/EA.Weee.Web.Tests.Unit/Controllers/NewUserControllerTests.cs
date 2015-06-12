namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using ViewModels.NewUser;
    using Web.Controllers;
    using Xunit;

    public class NewUserControllerTests
    {
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

        private static NewUserController GetMockNewUserController(object viewModel)
        {
            var registrationController = new NewUserController(() => new WeeeClient("test"));
            // Mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(viewModel, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModel, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                registrationController.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }

            return registrationController;
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
            };

            return validuserCreationViewModel;
        }
    }
}
