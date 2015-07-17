namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.PCS.MemberRegistration;

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

                return RedirectToAction("ViewErrorsAndWarnings", "MemberRegistration",
                    new { area = "PCS", memberUploadId = validationId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    return View("ViewErrorsAndWarnings",
                        new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors });
                }

                return View("XmlHasNoErrors",
                    new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors });
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