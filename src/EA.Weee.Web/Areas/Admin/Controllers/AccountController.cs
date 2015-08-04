namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Base;
    using Core;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using Thinktecture.IdentityModel.Client;
    using ViewModels;
    using UserInfoClient = Thinktecture.IdentityModel.Client.UserInfoClient;

    public class AccountController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IEmailService emailService;
        private readonly Func<IUserInfoClient> userInfoClient;

        public AccountController(Func<IWeeeClient> apiClient, IAuthenticationManager authenticationManager, IEmailService emailService, Func<IOAuthClient> oauthClient, Func<IUserInfoClient> userInfoClient)
        {
            this.apiClient = apiClient;
            this.oauthClient = oauthClient;
            this.authenticationManager = authenticationManager;
            this.emailService = emailService;
            this.userInfoClient = userInfoClient;
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
                    var signInResponse = await oauthClient().GetAccessTokenAsync(userCreationData.Email, userCreationData.Password);
                    authenticationManager.SignIn(signInResponse.GenerateUserIdentity());
                    bool emailSent = await Send(userCreationData.Email, userId, signInResponse.AccessToken);
                    if (!emailSent)
                    {
                        ViewBag.Errors = new[]
                        {
                            "Email is currently unavailable at this time, please try again later."
                        };
                    }
                }

                return RedirectToAction("AdminAccountActivationRequired", "Account", new { area = "Admin" });
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);

                if (ModelState.IsValid)
                {
                    throw;
                }
            }
            catch (SmtpException)
            {
                ViewBag.Errors = new[] { "The activation email was not sent, please try again later." };
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult AdminAccountActivationRequired()
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
        public async Task<ActionResult> AdminAccountActivationRequired(FormCollection model)
        {
            var email = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }
            try
            {
                //Resend activation email
                bool emailSent = await Send(email, User.GetUserId(), User.GetAccessToken());
                if (!emailSent)
                {
                    ViewBag.Errors = new[]
                    {
                        "Email is currently unavailable at this time, please try again later."
                    };
                }
            }
            catch (SmtpException)
            {
                ViewBag.Errors = new[] { "The activation email was not sent, please try again later." };
            }

            return RedirectToAction("AdminAccountActivationRequired", "Account", new { area = "Admin" });
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
                    return RedirectToAction("AdminAccountActivationRequired", "Account", new { area = "Admin" });
                }
            }

            return View();
        }

        private async Task<bool> Send(string email, string userId, string accessToken)
        {
            using (var client = apiClient())
            {
                var activationCode = await client.NewUser.GetUserAccountActivationTokenAsync(accessToken);

                if (Request.Url != null)
                {
                    var baseUrl = Url.Action("ActivateUserAccount", "Account", new { area = "Admin" }, Request.Url.Scheme);
                    var activationEmail = emailService.GenerateUserAccountActivationMessage(baseUrl, activationCode, userId, email);
                    return await emailService.SendAsync(activationEmail);
                }
            }
            return false;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(InternalLoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);

            if (response.AccessToken != null)
            {
                var isInternalUser = await IsInternalUser(response.AccessToken);
                if (isInternalUser)
                {
                    authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe },
                        response.GenerateUserIdentity());
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid login details");
                return View("Login", model);
            }

            ModelState.AddModelError(string.Empty, ParseLoginError(response.Error));

            return View("Login", model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        private async Task<bool> IsInternalUser(string accessToken)
        {
            var userInfo = await userInfoClient().GetUserInfoAsync(accessToken);

            return userInfo.Claims.Any(p => p.Item2 == Claims.CanAccessInternalArea);
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
    }
}