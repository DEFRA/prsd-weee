namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.PCS;

    public class MemberRegistrationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public MemberRegistrationController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ViewResult AddOrAmendMembers()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrAmendMembers(Guid id, HttpPostedFileBase file)
        {
            string xmlToValidate = FileToString(file);

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