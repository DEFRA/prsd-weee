namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Web.ApiClient;
    using EA.Prsd.Core.Web.Mvc.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.Shared;
    using ViewModels.PCS;

    [Authorize]
    public class PCSController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public PCSController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> ChooseActivity(Guid id)
        {
            var model = new ChooseActivityViewModel();
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                model.OrganisationId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(ChooseActivityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ActivityOptions.SelectedValue == "Add / amend PCS members")
                {
                    return RedirectToAction("ManagePCSMembers", new { id = viewModel.OrganisationId });
                }
                if (viewModel.ActivityOptions.SelectedValue == "Manage organisation users")
                {
                    return RedirectToAction("ManageOrganisationUsers", new { id = viewModel.OrganisationId });
                }
            }

            return View(viewModel);
        }
    }
}