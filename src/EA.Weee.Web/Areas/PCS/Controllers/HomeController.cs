namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.MemberRegistration;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.PCS;
    using Api.Client;
    using Infrastructure;
    using ViewModels;
    using Weee.Requests.Organisations;

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

        [HttpGet]
        public ViewResult AddOrAmendMembers()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddOrAmendMembers(HttpPostedFileBase file)
        {
            string xmlToValidate = FileToString(file);

            using (var client = apiClient())
            {
                var memberUploadId = await client.SendAsync(User.GetAccessToken(), new ValidateXmlFile(xmlToValidate));

                return RedirectToAction("ViewErrorsAndWarnings", new { id = memberUploadId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid id)
        {
            using (var client = apiClient())
            {
                var errors = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(id));

                return View(new MemberUploadResultViewModel { ErrorData = errors });
            }
        }

        private string FileToString(HttpPostedFileBase file)
        {
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);

            return Encoding.UTF8.GetString(binData);
        }
    }
}