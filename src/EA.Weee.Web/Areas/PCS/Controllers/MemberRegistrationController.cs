namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.ViewModels.PCS;

    public class MemberRegistrationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IFileConverterService fileConverter;

        public MemberRegistrationController(Func<IWeeeClient> apiClient, IFileConverterService fileConverter)
        {
            this.apiClient = apiClient;
            this.fileConverter = fileConverter;
        }

        [HttpGet]
        public async Task<ViewResult> AddOrAmendMembers(Guid id)
        {   
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));
                if (orgExists)
                {
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrAmendMembers(Guid id, HttpPostedFileBase file)
        {
            if (file == null)
            {
                ModelState.AddModelError(string.Empty, "Please select an XML file.");
                return View();
            }

            string xmlToValidate = fileConverter.Convert(file);

            using (var client = apiClient())
            {
                var memberUploadId = await client.SendAsync(User.GetAccessToken(), new ValidateXmlFile(id, xmlToValidate));

                return RedirectToAction("ViewErrorsAndWarnings", new { memberUploadId = memberUploadId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid id, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(memberUploadId));

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