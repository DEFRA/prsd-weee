namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.Organisations;
    using Weee.Requests.PCS.MemberRegistration;

    public class MemberRegistrationController : ExternalSiteController
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
                var validationId = await client.SendAsync(User.GetAccessToken(), new ProcessXMLFile(pcsId, fileData));

                return RedirectToAction("ViewErrorsAndWarnings", "MemberRegistration",
                    new { area = "PCS", memberUploadId = validationId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Summary(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var summary = await client.SendAsync(User.GetAccessToken(), new GetLatestMemberUploadList(pcsId));

                if (summary.LatestMemberUploads.Any())
                {
                    return View(SummaryViewModel.Create(summary.LatestMemberUploads));
                }
            }

            return RedirectToAction("AddOrAmendMembers", "MemberRegistration");
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));

                var memberUpload = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadById(memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    return View("ViewErrorsAndWarnings",
                        new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors, TotalCharges = memberUpload.TotalCharges });
                }

                return View("XmlHasNoErrors",
                    new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors, TotalCharges = memberUpload.TotalCharges });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitXml(Guid pcsId, MemberUploadResultViewModel viewModel)
        {
            using (var client = apiClient())
            {
                // TODO: insert request including check against submitting a member upload with errors or different PCS here...

                await client.SendAsync(User.GetAccessToken(), new MemberUploadSubmission(viewModel.MemberUploadId));

                return RedirectToAction("SuccessfulSubmission", new { memberUploadId = viewModel.MemberUploadId });
            }
        }

        [HttpGet]
        public ViewResult SuccessfulSubmission(Guid memberUploadId)
        {
            var model = new SuccessfulSubmissionViewModel { MemberUploadId = memberUploadId };
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetProducerCSV(Guid memberUploadId, string fileName = null)
        {
            using (var client = apiClient())
            {
                var producerCSVData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerCSVByMemberUploadId(memberUploadId));

                return File(new UTF8Encoding().GetBytes(producerCSVData.FileContent), "text/csv", fileName ?? producerCSVData.FileName);
            }
        }
    }
}