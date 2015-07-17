namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using ViewModels;
    using Weee.Requests.Shared;

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
        public async Task<ViewResult> AddOrAmendMembers(Guid pcsId)
        {   
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));
                if (orgExists)
                {
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", pcsId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrAmendMembers(Guid pcsId, AddOrAmendMembersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fileData = fileConverter.Convert(model.File);
            using (var client = apiClient())
            {
                var validationId = await client.SendAsync(User.GetAccessToken(), new ValidateXmlFile(pcsId, fileData));

                return RedirectToAction("ViewErrorsAndWarnings", "MemberRegistration", new { area = "PCS", memberUploadId = validationId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    return View("ViewErrorsAndWarnings", new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors });
                }

                return View("XmlHasNoErrors", new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitXml(Guid pcsId, MemberUploadResultViewModel viewModel)
        {
            using (var client = apiClient())
            {
                // TODO: insert request including check against submitting a member upload with errors or different PCS here...

                return RedirectToAction("SuccessfulSubmission");
            }
        }

        [HttpGet]
        public ViewResult SuccessfulSubmission()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetProducerCSV()
        {
            using (var client = apiClient())
            {
                //var producerCSVData = client.SendAsync(User.GetAccessToken(), new GetProducerCSVByComplianceYear())
                string csv = "Test, Test, Test";
                return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report123.csv");
            }
        }
    }
}