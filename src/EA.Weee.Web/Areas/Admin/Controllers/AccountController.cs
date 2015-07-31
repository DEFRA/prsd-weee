namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Net.Mail;
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
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;

        public AccountController(Func<IWeeeClient> apiClient, Func<IOAuthClient> oauthClient, IAuthenticationManager authenticationManager, IEmailService emailService)
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
                    await client.NewUser.CreateUserAsync(userCreationData);
                    var signInResponse = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);

                    authenticationManager.SignIn(signInResponse.GenerateUserIdentity());
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
        public async Task<ActionResult> UserAccountActivationRequired(FormCollection model)
        {
            var email = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }
            try
            {
                using (var client = apiClient())
                {
                    //Resend activation email
                    string activationToken = await client.NewUser.GetUserAccountActivationTokenAsync(User.GetAccessToken());
                    string verificationBaseUrl = Url.Action("ActivateUserAccount", "Account", new { area = "Admin" });
                    string userid = User.GetUserId();
                    MailMessage activationEmail = emailService.GenerateUserAccountActivationMessage(verificationBaseUrl, activationToken, userid, email);
                    bool emailSent = await emailService.SendAsync(activationEmail);
                    if (!emailSent)
                    {
                        ViewBag.Errors = new[]
                            {
                                "Email is currently unavailable at this time, please try again later."
                            };
                        return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
                    }
                }
            }
            catch (SmtpException)
            {
                ViewBag.Errors = new[] { "The activation email was not sent, please try again later." };
                return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
            }

            return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateUserAccount(Guid id, string code)
        {
            using (var client = apiClient())
            {
                bool result =
                    await
                        client.NewUser.ActivateUserAccountEmailAsync(new ActivatedUserAccountData
                        {
                            Id = id,
                            Code = code
                        });

                if (!result)
                {
                    return RedirectToAction("UserAccountActivationRequired", "Account", new { area = "Admin" });
                }
            }

            return View();
        }
    }
}