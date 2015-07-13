namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using Services;
    using ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.PCS.MemberRegistration;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public HomeController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> ChooseActivity(Guid pcsId)
        {
            var model = new ChooseActivityViewModel();
            using (var client = apiClient())
            {
                var organisationExists =
                    await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));

                if (!organisationExists)
                {
                    throw new ArgumentException("No organisation found for supplied organisation Id", "organisationId");
                }

                model.OrganisationId = pcsId;
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
                    return RedirectToAction("AddOrAmendMembers", "MemberRegistration", new { pcsId = viewModel.OrganisationId });
                }
                if (viewModel.ActivityOptions.SelectedValue == PcsAction.ManageOrganisationUsers)
                {
                    return RedirectToAction("ManageOrganisationUsers", new { pcsId = viewModel.OrganisationId });
                }
            }

            return View(viewModel);
        }
    }
}