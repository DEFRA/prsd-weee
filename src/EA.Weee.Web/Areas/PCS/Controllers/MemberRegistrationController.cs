namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.Globalization;
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

                return RedirectToAction("SuccessfulSubmission", new { memberUploadId = viewModel.MemberUploadId });
            }
        }

        [HttpGet]
        public ViewResult SuccessfulSubmission(Guid memberUploadId)
        {
            ViewBag.MemberUploadId = memberUploadId;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetProducerCSV(Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var producerCSVData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerCSVByMemberUploadId(memberUploadId));

                var memberUpload = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadById(memberUploadId));

                var csvFileName = DateTime.Now.ToString(CultureInfo.InvariantCulture) + "-" + memberUpload.ComplianceYear.ToString() + ".csv";

                return File(new System.Text.UTF8Encoding().GetBytes(producerCSVData), "text/csv", csvFileName);
            }
        }
    }
}