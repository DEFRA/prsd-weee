namespace EA.Weee.Web.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.MemberRegistration;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.TemporaryXmlTest;

    public class TemporaryXmlTestController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public TemporaryXmlTestController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("AddOrAmendMembers");
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

                return RedirectToAction("TestViewingUpload", new { id = memberUploadId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> TestViewingUpload(Guid id)
        {
            using (var client = apiClient())
            {
                var errors = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(id));

                return View(new TestViewingUploadViewModel { ErrorsWithLevels = errors });
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