namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Api.Client.Entities;
    using Infrastructure;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Requests.Organisations;
    using ViewModels.JoinOrganisation;
    using ViewModels.NewUser;
    using ViewModels.Shared;

    [Authorize]
    public class NewUserController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public NewUserController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult CheckUserAccountCreation()
        {
            return View(new YesNoChoiceViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckUserAccountCreation(YesNoChoiceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var selectedOption = viewModel.Choices.SelectedValue;
            if (selectedOption.Equals("No"))
            {
                return RedirectToAction("CheckIsPcs");
            }

            return RedirectToAction("Login", "Account");
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
                        await client.NewUser.CreateUserAsync(userCreationData);
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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SelectOrganisation(string organisationName)
        {
            var model = new SelectOrganisationViewModel
            {
                Name = organisationName
            };

            if (string.IsNullOrEmpty(organisationName))
            {
                model.Organisations = null;
            }
            else
            {
                using (var client = apiClient())
                {
                    var response =
                        await client.SendAsync(User.GetAccessToken(), new FindMatchingOrganisations(organisationName));

                    model.Organisations = response;
                }
            }

            return View("SelectOrganisation", model);
        }
    }
}