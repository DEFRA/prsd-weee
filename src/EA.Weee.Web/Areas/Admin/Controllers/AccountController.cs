namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Authorization;
    using Base;
    using Core.Routing;
    using Core.Shared;
    using Extensions;
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
                    var loginResult = await weeeAuthorization.SignIn(model.Email, model.Password, false);

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

                AddRemoveModelStateErrors();
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

            return View("AccountActivationRequired");
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminAccountActivationFailed(Guid id, AccountActivationRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountActivationFailed", model);
            }

            using (var client = apiClient())
            {
                string activationBaseUrl = externalRouteService.ActivateInternalUserAccountUrl;

                var result = await client.User.ResendActivationEmailByUserId(id.ToString(), model.Email, activationBaseUrl);

                if (!result)
                {
                    ModelState.AddModelError("Email", "The email address does not match the address for your account.");
                    return View("AccountActivationFailed", model);
                }
                else
                {
                    ViewBag.Email = model.Email;
                    return View("AccountActivationRequested");
                }
            }
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
                    return View("AccountActivationFailed");
                }
                else
                {
                    return View("AccountActivated");
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(string returnUrl)
        {
            var authorizationState = await weeeAuthorization.GetAuthorizationState();

            if (authorizationState.IsLoggedIn)
            {
                return this.LoginRedirect(authorizationState.DefaultLoginAction, returnUrl);
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
                await weeeAuthorization.SignIn(model.Email, model.Password, model.RememberMe);

                if (loginResult.Successful)
                {
                    return this.LoginRedirect(loginResult.DefaultLoginAction, returnUrl);
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

        [HttpGet]
        public ActionResult InternalUserAuthorisationRequired(UserStatus? userStatus)
        {
            if (!userStatus.HasValue)
            {
                throw new InvalidOperationException("User status not recognised");
            }

            if (userStatus == UserStatus.Active)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View(new InternalUserAuthorizationRequiredViewModel { Status = userStatus.Value });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(Guid id, string token)
        {
            using (var client = apiClient())
            {
                var passwordResetData = new PasswordResetData
                {
                    Password = string.Empty,
                    Token = token,
                    UserId = id
                };

                bool result = await client.User.IsPasswordResetTokenValidAsync(passwordResetData);
                return View(!result ? "ResetPasswordExpired" : "ResetPassword");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(Guid id, string token, ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                var passwordResetData = new PasswordResetData
                {
                    Password = model.Password,
                    Token = token,
                    UserId = id
                };

                try
                {
                    bool result = await client.User.ResetPasswordAsync(passwordResetData);

                    return View(!result ? "ResetPasswordExpired" : "ResetPasswordComplete");
                }
                catch (ApiBadRequestException ex)
                {
                    this.HandleBadRequest(ex);

                    if (ModelState.IsValid)
                    {
                        throw;
                    }
                    AddRemoveModelStateErrors();
                }
                return View(model);
            }
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
                ViewBag.Email = model.Email;
                return View("ResetPasswordInstruction");
            }
        }

        private void AddRemoveModelStateErrors()
        {
            foreach (var modelState in ViewData.ModelState.Values.ToList())
            {
                List<int> errorsToRemoveIndex = new List<int>();
                List<String> errorsToAdd = new List<string>();
                for (var i = modelState.Errors.Count - 1; i >= 0; i--)
                {
                    if (modelState.Errors[i].ErrorMessage.Contains("Passwords") && modelState.Value == null)
                    {
                        errorsToRemoveIndex.Add(i);
                    }
                    else if (modelState.Errors[i].ErrorMessage.Contains("is already taken"))
                    {
                        errorsToRemoveIndex.Add(i);
                        errorsToAdd.Add("An account already exists with this email address. Sign in or reset your password.");
                    }
                }
                foreach (int index in errorsToRemoveIndex)
                {
                    modelState.Errors.RemoveAt(index);
                }
                foreach (string error in errorsToAdd)
                {
                    modelState.Errors.Add(error);
                }
            }
        }
    }
}