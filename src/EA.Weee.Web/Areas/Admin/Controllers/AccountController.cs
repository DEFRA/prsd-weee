namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Authorization;
    using Base;
    using Core.Routing;
    using Core.Shared;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Services;
    using ViewModels.Account;
    using Weee.Requests.Admin;

    public class AccountController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IExternalRouteService externalRouteService;
        private readonly IWeeeAuthorization weeeAuthorization;

        public AccountController(
            Func<IWeeeClient> apiClient,
            IAuthenticationManager authenticationManager,
            IExternalRouteService externalRouteService,
            IWeeeAuthorization weeeAuthorization)
        {
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.externalRouteService = externalRouteService;
            this.weeeAuthorization = weeeAuthorization;
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

            var userCreationData = new InternalUserCreationData
            {
                Email = model.Email,
                FirstName = model.Name,
                Surname = model.Surname,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                ActivationBaseUrl = externalRouteService.ActivateInternalUserAccountUrl,
            };

            try
            {
                using (var client = apiClient())
                {
                    var userId = await client.User.CreateInternalUserAsync(userCreationData);
                    var loginResult = await weeeAuthorization.SignIn(LoginType.Internal, model.Email, model.Password, false);

                    if (loginResult.Successful)
                    {
                        await client.SendAsync(loginResult.AccessToken, new AddCompetentAuthorityUser(userId));
                        return RedirectToAction("AdminAccountActivationRequired");
                    }

                    ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
                }               
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
            ViewBag.UserEmailAddress = User.GetEmailAddress();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminAccountActivationRequired(FormCollection model)
        {
            using (var client = apiClient())
            {
                string accessToken = authenticationManager.User.GetAccessToken();

                string activationBaseUrl = externalRouteService.ActivateInternalUserAccountUrl;

                await client.User.ResendActivationEmail(accessToken, activationBaseUrl);
            }

            return View("AccountActivationRequested");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateUserAccount(Guid id, string code)
        {
            using (var client = apiClient())
            {
                bool result =
                    await
                        client.User.ActivateUserAccountEmailAsync(new ActivatedUserAccountData
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("SignIn");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(InternalLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var loginResult =
                await weeeAuthorization.SignIn(LoginType.Internal, model.Email, model.Password, model.RememberMe);

                if (loginResult.Successful)
                {
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
            }

            return View(model);
        }

        // POST: /Admin/Account/SignOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            weeeAuthorization.SignOut();

            return RedirectToAction("SignIn");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<ActionResult> InternalUserAuthorisationRequired()
        {
            using (var client = apiClient())
            {
                var status = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                if (status == UserStatus.Active)
                {
                    return RedirectToAction("ChooseActivity", "Home", new { area = "Admin" });
                }

                InternalUserAuthorizationRequiredViewModel model = new InternalUserAuthorizationRequiredViewModel() { Status = status };
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(Guid id, string token)
        {
            return View(new ResetPasswordModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(Guid id, string token, ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = apiClient())
                    {
                        await client.User.ResetPasswordAsync(new PasswordResetData
                        {
                            Password = model.Password,
                            Token = token,
                            UserId = id
                        });

                        return RedirectToAction("SignIn");
                    }
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordRequest()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPasswordRequest(ResetPasswordRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                ResetPasswordRoute route = externalRouteService.InternalUserResetPasswordRoute;
                PasswordResetRequest apiModel = new PasswordResetRequest(model.Email, route);

                var result = await client.User.ResetPasswordRequestAsync(apiModel);

                if (!result.ValidEmail)
                {
                    ModelState.AddModelError("Email", "Email address not recognised.");
                    return View(model);
                }
                else
                {
                    ViewBag.Email = model.Email;
                    return View("ResetPasswordInstruction");
                }
            }
        }
    }
}