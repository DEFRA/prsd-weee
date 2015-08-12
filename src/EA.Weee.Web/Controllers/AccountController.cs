namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Core;
    using Core.Organisations;
    using Core.Shared;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using Thinktecture.IdentityModel.Client;
    using ViewModels.Account;
    using ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IEmailService emailService;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly Func<IUserInfoClient> userInfoClient;

        public AccountController(Func<IOAuthClient> oauthClient,
            IAuthenticationManager authenticationManager,
            Func<IWeeeClient> apiClient,
            IEmailService emailService,
            Func<IUserInfoClient> userInfoClient)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.emailService = emailService;
            this.userInfoClient = userInfoClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);
            if (response.AccessToken != null)
            {
                var isExternalUser = await IsExternalUser(response.AccessToken);
                if (isExternalUser)
                {
                    authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe },
                        response.GenerateUserIdentity());
                    return RedirectToLocal(model.ReturnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid login details");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, ParseLoginError(response.Error));

            return View(model);
        }

        private string ParseLoginError(string error)
        {
            switch (error)
            {
                case OAuth2Constants.Errors.AccessDenied:
                    return "Access denied";
                case OAuth2Constants.Errors.InvalidGrant:
                    return "Invalid credentials";
                case OAuth2Constants.Errors.Error:
                case OAuth2Constants.Errors.InvalidClient:
                case OAuth2Constants.Errors.InvalidRequest:
                case OAuth2Constants.Errors.InvalidScope:
                case OAuth2Constants.Errors.UnauthorizedClient:
                case OAuth2Constants.Errors.UnsupportedGrantType:
                case OAuth2Constants.Errors.UnsupportedResponseType:
                default:
                    return "Internal error";
            }
        }

        private async Task<bool> IsExternalUser(string accessToken)
        {
            var userInfo = await userInfoClient().GetUserInfoAsync(accessToken);

            return userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessExternalArea);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("RedirectProcess");
        }

        [HttpGet]
        public async Task<ActionResult> RedirectProcess()
        {
            using (var client = apiClient())
            {
                var approvedOrganisationUsers = await
                    client.SendAsync(
                        User.GetAccessToken(),
                        new GetOrganisationsByUserId(User.GetUserId(), new[] { (int)UserStatus.Approved }));

                var inactiveOrganisationUsers = await
                    client.SendAsync(
                        User.GetAccessToken(),
                        new GetOrganisationsByUserId(User.GetUserId(),
                            new[]
                            {
                                (int)UserStatus.Pending, (int)UserStatus.Refused,
                                (int)UserStatus.Inactive
                            }));

                if (approvedOrganisationUsers.Count >= 1)
                {
                    return RedirectToAction("ChooseActivity", "Home",
                        new
                        {
                            area = "Scheme",
                            pcsId = approvedOrganisationUsers.First().OrganisationId,
                        });
                }
                else if (inactiveOrganisationUsers.Count >= 1)
                {
                    return RedirectToAction("HoldingMessageForPending", "Organisation");
                }
                else
                {
                    return RedirectToAction("Type", "OrganisationRegistration");
                }
            }
        }

        [HttpGet]
        public ActionResult UserAccountActivationRequired()
        {
            string email = User.GetEmailAddress();
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
            string email = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }
            try
            {
                using (var client = apiClient())
                {
                    var activationToken =
                        await client.NewUser.GetUserAccountActivationTokenAsync(User.GetAccessToken());
                    var activationEmail =
                        emailService.GenerateUserAccountActivationMessage(
                            Url.Action("ActivateUserAccount", "Account", null, Request.Url.Scheme),
                            activationToken, User.GetUserId(), User.GetEmailAddress());
                    var emailSent = await emailService.SendAsync(activationEmail);

                    if (!emailSent)
                    {
                        ViewBag.Errors = new[]
                        {
                            "Email is currently unavailable at this time, please try again later."
                        };
                        return View();
                    }
                }
            }
            catch (SmtpException)
            {
                ViewBag.Errors = new[] { "The activation email was not sent, please try again later." };
                return View();
            }

            return RedirectToAction("UserAccountActivationRequired");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateUserAccount(Guid id, string code)
        {
            using (var client = apiClient())
            {
                bool result = await client.NewUser.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code });

                if (!result)
                {
                    return RedirectToAction("UserAccountActivationRequired");
                }
            }

            return View();
        }
    }
}