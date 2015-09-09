namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Api.Client.Entities;
    using Authorization;
    using EA.Weee.Core.Routing;
    using EA.Weee.Web.Controllers.Base;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Account;

    [Authorize]
    public class AccountController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IExternalRouteService externalRouteService;

        public AccountController(Func<IWeeeClient> apiClient,
            IWeeeAuthorization weeeAuthorization,
            IExternalRouteService externalRouteService)
        {
            this.apiClient = apiClient;
            this.weeeAuthorization = weeeAuthorization;
            this.externalRouteService = externalRouteService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
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
        public async Task<ActionResult> SignIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await weeeAuthorization.SignIn(LoginType.External, model.Email, model.Password, model.RememberMe);

                if (loginResult.Successful)
                {
                    return RedirectToLocal(model.ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
            }

            return View(model);
        }

        // POST: /Account/SignOut
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

            return RedirectToAction("RedirectProcess");
        }

        [HttpGet]
        public ActionResult RedirectProcess()
        {
            return RedirectToRoute("SelectOrganisation");
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
            string emailAddress = User.GetEmailAddress();
            if (!string.IsNullOrEmpty(emailAddress))
            {
                ViewBag.UserEmailAddress = User.GetEmailAddress();
            }

            using (var client = apiClient())
            {
                string accessToken = User.GetAccessToken();

                string activationBaseUrl = externalRouteService.ActivateExternalUserAccountUrl;

                await client.User.ResendActivationEmail(accessToken, activationBaseUrl);
            }

            return RedirectToAction("UserAccountActivationRequired");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateUserAccount(Guid id, string code)
        {
            using (var client = apiClient())
            {
                bool result = await client.User.ActivateUserAccountEmailAsync(new ActivatedUserAccountData { Id = id, Code = code });

                if (!result)
                {
                    return RedirectToAction("UserAccountActivationRequired");
                }
            }

            return View();
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
                        var result = await client.User.ResetPasswordAsync(new PasswordResetData
                        {
                            Password = model.Password,
                            Token = token,
                            UserId = id
                        });

                        var loginResult =
                            await weeeAuthorization.SignIn(LoginType.External, result.EmailAddress, model.Password, false);

                        if (loginResult.Successful)
                        {
                            return RedirectToAction("RedirectProcess", "Account");
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
                ResetPasswordRoute route = externalRouteService.ExternalUserResetPasswordRoute;
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