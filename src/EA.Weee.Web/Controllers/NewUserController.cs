namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Services;
    using ViewModels.NewUser;
    using ViewModels.Shared;

    [Authorize]
    public class NewUserController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;
        private readonly Func<IOAuthClient> oauthClient;

        public NewUserController(Func<IOAuthClient> oauthClient,
            Func<IWeeeClient> apiClient,
            IAuthenticationManager authenticationManager,
            IEmailService emailService)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult CheckIsPcs()
        {
            return View(new YesNoChoiceViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIsPcs(YesNoChoiceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var selectedOption = viewModel.Choices.SelectedValue;

            if (selectedOption.Equals("No"))
            {
                return RedirectToAction("AccountNotRequired");
            }

            if (selectedOption.Equals("Yes"))
            {
                return RedirectToAction("CheckComplianceYear");
            }

            throw new ArgumentException("Unexpected argument value received, expected Yes or No");
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult CheckComplianceYear()
        {
            return View(new YesNoChoiceViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckComplianceYear(YesNoChoiceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var selectedOption = viewModel.Choices.SelectedValue;

            if (selectedOption.Equals("No"))
            {
                return RedirectToAction("AccountNotRequired");
            }

            if (selectedOption.Equals("Yes"))
            {
                return RedirectToAction("UserCreation");
            }

            throw new ArgumentException("Unexpected argument value received, expected Yes or No");
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult AccountNotRequired()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UserCreation()
        {
            return View(new UserCreationViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserCreation(UserCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var userCreationData = new UserCreationData
                    {
                        Email = model.Email,
                        FirstName = model.Name,
                        Surname = model.Surname,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword
                    };

                    try
                    {
                        var userId = await client.NewUser.CreateUserAsync(userCreationData);
                       
                        var signInResponse = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);
                        
                        authenticationManager.SignIn(signInResponse.GenerateUserIdentity());

                        var verificationCode = await client.NewUser.GetUserEmailVerificationTokenAsync(signInResponse.AccessToken);
                        
                        var verificationEmail = emailService.GenerateEmailVerificationMessage(Url.Action("VerifyEmail", "Account", null, Request.Url.Scheme), verificationCode, userId, model.Email);
                        
                        await emailService.SendAsync(verificationEmail);

                        return RedirectToAction("Confirm", "NewUser");
                    }
                    catch (ApiBadRequestException ex)
                    {
                        this.HandleBadRequest(ex);

                        if (ModelState.IsValid)
                        {
                            throw;
                        }
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            return View();
        }
    }
}