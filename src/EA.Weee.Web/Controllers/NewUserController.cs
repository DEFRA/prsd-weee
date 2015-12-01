namespace EA.Weee.Web.Controllers
{
    using Api.Client;
    using Api.Client.Entities;
    using Core;
    using Infrastructure;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Prsd.Core.Web.OAuth;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.NewUser;
    using ViewModels.Shared;

    [Authorize]
    public class NewUserController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IExternalRouteService externalRouteService;
        private readonly IAppConfiguration appConfig;

        public NewUserController(Func<IOAuthClient> oauthClient,
            Func<IWeeeClient> apiClient,
            IAuthenticationManager authenticationManager,
            IExternalRouteService externalRouteService,
            IAppConfiguration appConfig)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.externalRouteService = externalRouteService;
            this.appConfig = appConfig;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UserCreation()
        {
            // If the user is currently signed-in, we don't want to confuse them
            // by showing their current user name whilst they create a new user.
            // Therefore we need to sign them out and clear the user from the
            // current HttpContet.
            authenticationManager.SignOut();
            WindowsIdentity identity = WindowsIdentity.GetAnonymous();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            HttpContext.User = principal;

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
                    var userCreationData = new ExternalUserCreationData
                    {
                        Email = model.Email,
                        FirstName = model.Name,
                        Surname = model.Surname,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        ActivationBaseUrl = externalRouteService.ActivateExternalUserAccountUrl,
                    };

                    try
                    {
                        var userId = await client.User.CreateExternalUserAsync(userCreationData);

                        var signInResponse = await oauthClient().GetAccessTokenAsync(model.Email, model.Password);

                        authenticationManager.SignIn(signInResponse.GenerateUserIdentity());

                        return RedirectToAction("UserAccountActivationRequired", "Account");
                    }
                    catch (ApiBadRequestException ex)
                    {
                        this.HandleBadRequest(ex);

                        if (ModelState.IsValid)
                        {
                            throw;
                        }
                      
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
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Cookies()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Feedback()
        {
            if (!string.IsNullOrEmpty(appConfig.DonePageUrl))
            {
                return Redirect(appConfig.DonePageUrl);
            }

            var collection = new List<string>
            {
                FeedbackOptions.VerySatisfied, 
                FeedbackOptions.Satisfied, 
                FeedbackOptions.NeitherSatisfiedOrDissatisfied,
                FeedbackOptions.Dissatisfied,
                FeedbackOptions.VeryDissatisfied
            };

            var model = new FeedbackViewModel
            {
                PossibleValues = collection
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View(model);
        }
    }
}