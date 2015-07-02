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
    using Weee.Requests.MemberRegistration;
    using Weee.Requests.Organisations;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IFileConverter fileConverter;

        public HomeController(Func<IWeeeClient> apiClient, IFileConverter fileConverter)
        {
            this.apiClient = apiClient;
            this.fileConverter = fileConverter;
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

        [HttpGet]
        public async Task<ActionResult> ManageMembers(Guid id)
        {
            using (var client = apiClient())
            {
                var clientExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));
                if (clientExists)
                {
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", id));
        }

        [HttpPost]
        public async Task<ActionResult> ManageMembers(Guid id, HttpPostedFileBase file)
        {
            var fileData = fileConverter.Convert(file);
            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new ValidateXMLFile(id, fileData));
            }

            throw new NotImplementedException();
        }
    }
}