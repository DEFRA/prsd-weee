namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Areas.PCS.ViewModels;
    using EA.Weee.Web.Infrastructure;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public HomeController(Func<IWeeeClient> apiClient)
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
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManagePcsMembers)
                {
                    return RedirectToAction("ManagePCSMembers", new { id = viewModel.OrganisationId });
                }
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManageOrganisationUsers)
                {
                    return RedirectToAction("ManageOrganisationUsers", new { id = viewModel.OrganisationId });
                }
            }

            return View(viewModel);
        }
    }
}