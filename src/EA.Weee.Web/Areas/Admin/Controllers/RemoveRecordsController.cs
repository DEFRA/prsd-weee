namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Services;
    using Infrastructure;
    using Security;
    using Weee.Requests.Admin;

    public class RemoveRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IAppConfiguration configuration;

        public RemoveRecordsController(
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
                        return RedirectToAction("ChooseActivity");
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
            RemoveRecordsViewModel viewModel = new RemoveRecordsViewModel();
            PopulateViewModelPossibleValues(viewModel);
            return View("ChooseActivity", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(RemoveRecordsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewModelPossibleValues(viewModel);
                return View(viewModel);
            }

            switch (viewModel.SelectedValue)
            {
                case InternalRemoveRecordsActivity.RemovePCS:
                    return RedirectToAction("RemovePCS");

                //case InternalRemoveRecordsActivity.RemoveAATF:
                //    return RedirectToAction("RemoveAATF");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public ActionResult RemovePCS(RemoveRecordsViewModel viewModel)
        {
            return View(viewModel);
        }

        private void PopulateViewModelPossibleValues(RemoveRecordsViewModel viewModel)
        {
            var isAdmin = new ClaimsPrincipal(User).HasClaim(p => p.Value == Claims.InternalAdmin);

            viewModel.PossibleValues = new List<string>();

            viewModel.PossibleValues.Add(InternalRemoveRecordsActivity.RemovePCS);
            //viewModel.PossibleValues.Add(InternalRemoveRecordsActivity.RemoveAATF);
        }
    }
}