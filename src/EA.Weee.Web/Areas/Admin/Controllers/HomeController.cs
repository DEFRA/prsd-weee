namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Security;
    using Infrastructure;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Home;
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
                            $"Cannot determine result for user with status '{userStatus}'");
                }
            }
        }

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            ChooseActivityViewModel viewModel = new ChooseActivityViewModel();
            PopulateViewModelPossibleValues(viewModel);
            return View("ChooseActivity", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(ChooseActivityViewModel viewModel)
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

                case InternalUserActivity.ManageAatfs:
                    return RedirectToAction("ManageAatfs", "Aatf", new { facilityType = FacilityType.Aatf });

                case InternalUserActivity.ManageAes:
                    return RedirectToAction("ManageAatfs", "Aatf", new { facilityType = FacilityType.Ae });

                case InternalUserActivity.ProducerDetails:
                    return RedirectToAction("Search", "Producers");

                case InternalUserActivity.ManageEvidenceNotes:
                    return RedirectToAction("Index", "Holding");

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

                case InternalUserActivity.ManagePcsObligations:
                    if (!configuration.EnablePCSObligations)
                    {
                        throw new InvalidOperationException("Obligations are not enabled.");
                    }
                    else
                    {
                        return RedirectToAction("SelectAuthority", "Obligations");
                    }

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

        private void PopulateViewModelPossibleValues(ChooseActivityViewModel viewModel)
        {
            var isAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);

            viewModel.PossibleValues = new List<string>();

            viewModel.PossibleValues.Add(InternalUserActivity.ManageScheme);
            viewModel.PossibleValues.Add(InternalUserActivity.SubmissionsHistory);
            viewModel.PossibleValues.Add(InternalUserActivity.ProducerDetails);
            viewModel.PossibleValues.Add(InternalUserActivity.ManageEvidenceNotes);

            if (configuration.EnablePCSObligations && isAdmin)
            {
                viewModel.PossibleValues.Add(InternalUserActivity.ManagePcsObligations);
            }
           
            if (configuration.EnableInvoicing)
            {
                viewModel.PossibleValues.Add(InternalUserActivity.ManagePcsCharges);
            }
            viewModel.PossibleValues.Add(InternalUserActivity.ManageAatfs);
            viewModel.PossibleValues.Add(InternalUserActivity.ManageAes);
            viewModel.PossibleValues.Add(InternalUserActivity.ManageUsers);
            viewModel.PossibleValues.Add(InternalUserActivity.ViewReports);
        }
    }
}
