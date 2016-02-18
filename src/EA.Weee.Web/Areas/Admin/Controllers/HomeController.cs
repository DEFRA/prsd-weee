namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using ViewModels.Home;
    using Web.ViewModels.Shared;
    using Weee.Requests.Admin;

    public class HomeController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAppConfiguration configuration;

        public HomeController(
            Func<IWeeeClient> apiClient,
            IAppConfiguration configuration)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
        }

        // GET: Admin/Home
        public async Task<ActionResult> Index()
        {
            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        return RedirectToAction("ChooseActivity", "Home");
                    case UserStatus.Inactive:
                    case UserStatus.Pending:
                    case UserStatus.Rejected:
                        return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { userStatus });
                    default:
                        throw new NotSupportedException(
                            string.Format("Cannot determine result for user with status '{0}'", userStatus));
                }
            }
        }

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            RadioButtonStringCollectionViewModel viewModel = new RadioButtonStringCollectionViewModel();
            PopulateViewModelPossibleValues(viewModel);
            return View("ChooseActivity", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(RadioButtonStringCollectionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewModelPossibleValues(viewModel);
                return View(viewModel);
            }

            switch (viewModel.SelectedValue)
            {
                case InternalUserActivity.ManageUsers:
                    return RedirectToAction("Index", "User");

                case InternalUserActivity.ManageScheme:
                    return RedirectToAction("ManageSchemes", "Scheme");

                case InternalUserActivity.ProducerDetails:
                    return RedirectToAction("Search", "Producers");

                case InternalUserActivity.SubmissionsHistory:
                    if (configuration.EnableDataReturns)
                    {
                        return RedirectToAction("ChooseSubmissionType", "Submissions");
                    }
                    else
                    {
                        return RedirectToAction("SubmissionsHistory", "Submissions");
                    }
                case InternalUserActivity.ViewReports:
                    return RedirectToAction("ChooseReport", "Reports");

                case InternalUserActivity.ManagePcsCharges:
                    {
                        if (!configuration.EnableInvoicing)
                        {
                            throw new InvalidOperationException("Invoicing is not enabled.");
                        }
                        else
                        {
                            return RedirectToAction("SelectAuthority", "Charge");
                        }
                    }

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateViewModelPossibleValues(RadioButtonStringCollectionViewModel viewModel)
        {
            viewModel.PossibleValues = new List<string>();

            viewModel.PossibleValues.Add(InternalUserActivity.ManageScheme);
            viewModel.PossibleValues.Add(InternalUserActivity.SubmissionsHistory);
            viewModel.PossibleValues.Add(InternalUserActivity.ProducerDetails);
            viewModel.PossibleValues.Add(InternalUserActivity.ManageUsers);
            viewModel.PossibleValues.Add(InternalUserActivity.ViewReports);

            if (configuration.EnableInvoicing)
            {
                viewModel.PossibleValues.Add(InternalUserActivity.ManagePcsCharges);
            }
        }
    }
}