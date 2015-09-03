﻿namespace EA.Weee.Web.Controllers
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
    using ViewModels.NewUser;
    using ViewModels.Shared;

    [Authorize]
    public class NewUserController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAuthenticationManager authenticationManager;
        private readonly Func<IOAuthClient> oauthClient;
        private readonly IExternalRouteService externalRouteService;

        public NewUserController(Func<IOAuthClient> oauthClient,
            Func<IWeeeClient> apiClient,
            IAuthenticationManager authenticationManager,
            IExternalRouteService externalRouteService)
        {
            this.oauthClient = oauthClient;
            this.apiClient = apiClient;
            this.authenticationManager = authenticationManager;
            this.externalRouteService = externalRouteService;
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
        public ActionResult RedirectToHomePage()
        {
            return RedirectToAction("LandingPage", "Home");
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
                        ConfirmPassword = model.ConfirmPassword,
                        Claims = new[]
                        {
                            Claims.CanAccessExternalArea
                        },
                        ActivationBaseUrl = externalRouteService.ActivateExternalUserAccountUrl,
                    };

                    try
                    {
                        var userId = await client.User.CreateUserAsync(userCreationData);

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
    }
}