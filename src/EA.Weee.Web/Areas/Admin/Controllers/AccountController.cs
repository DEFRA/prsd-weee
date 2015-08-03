namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Core;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Services;
    using ViewModels;

    public class AccountController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;
        private readonly Func<IOAuthClient> oauthClient;

        public AccountController(Func<IWeeeClient> apiClient, IAuthenticationManager authenticationManager, IEmailService emailService, Func<IOAuthClient> oauthClient)
        {
            this.apiClient = apiClient;
            this.oauthClient = oauthClient;
            this.authenticationManager = authenticationManager;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View(new InternalUserCreationViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InternalUserCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userCreationData = new UserCreationData
            {
                Email = model.Email, 
                FirstName = model.Name, 
                Surname = model.Surname, 
                Password = model.Password, 
                ConfirmPassword = model.ConfirmPassword, 
                Claims = new[]
                {
                    Claims.CanAccessInternalArea
                }
            };

            try
            {
                using (var client = apiClient())
                {
                    var userId = await client.NewUser.CreateUserAsync(userCreationData);
                    await SendEmail(userCreationData.Email, userCreationData.Password, client, userId);
                }

                return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
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

        public async Task<bool> SendEmail(string email, string password, IWeeeClient client, string userId)
        {
            var signInResponse = await oauthClient().GetAccessTokenAsync(email, password);

            authenticationManager.SignIn(signInResponse.GenerateUserIdentity());

            var activationCode = await client.NewUser.GetUserAccountActivationTokenAsync(signInResponse.AccessToken);

            string baseUrl = Url.Action("ActivateUserAccount", "Account", new { area = string.Empty }, Request.Url.Scheme);

            var activationEmail = emailService.GenerateUserAccountActivationMessage(baseUrl, activationCode, userId, email);

            return await emailService.SendAsync(activationEmail);
        }

        [HttpGet]
        public ActionResult UserAccountActivationRequired()
        {
            var email = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAccountActivationRequired(FormCollection model)
        {
            var email = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }
            //TODO Resend activation email

            return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
        }
    }
}