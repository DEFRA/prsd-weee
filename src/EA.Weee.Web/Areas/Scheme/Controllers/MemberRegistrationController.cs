namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;

    public class MemberRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IMapper mapper;
        private const string ManageMembersActivity = "Manage members";

        public MemberRegistrationController(
            Func<IWeeeClient> apiClient,
            IWeeeCache cache,
            BreadcrumbService breadcrumb,
            CsvWriterFactory csvWriterFactory,
            IMapper mapper)
        {
            this.apiClient = apiClient;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
            this.csvWriterFactory = csvWriterFactory;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> AuthorisationRequired(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId));

                if (status == SchemeStatus.Approved)
                {
                    return RedirectToAction("Summary", "MemberRegistration");
                }

                var userIdString = User.GetUserId();
                var showLinkToSelectOrganisation = false;

                if (userIdString != null)
                {
                    var userId = new Guid(userIdString);

                    var task = Task.Run(async () =>
                    {
                        return await cache.FetchUserActiveCompleteOrganisationCount(userId);
                    });
                    task.Wait();

                    showLinkToSelectOrganisation = (task.Result > 1);
                }

                await SetBreadcrumb(pcsId, ManageMembersActivity);
                return View(new AuthorizationRequiredViewModel
                {
                    Status = status,
                    ShowLinkToSelectOrganisation = showLinkToSelectOrganisation
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Summary(Guid pcsId)
        {
            using (var client = apiClient())
            {
                List<int> years = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(pcsId));

                if (years.Count > 0)
                {
                    await SetBreadcrumb(pcsId, ManageMembersActivity);
                    return View(years);
                }
            }

            return RedirectToAction("AddOrAmendMembers", "MemberRegistration");
        }

        [HttpGet]
        public async Task<ViewResult> AddOrAmendMembers(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));
                if (orgExists)
                {
                    await SetBreadcrumb(pcsId, ManageMembersActivity);
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", pcsId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrAmendMembers(Guid pcsId, PCSFileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(500);
                }
                else
                {
                    await SetBreadcrumb(pcsId, ManageMembersActivity);
                    return View(model);
                }
            }

            Guid validationId;

            using (var client = apiClient())
            {
                model.PcsId = pcsId;
                var request = mapper.Map<PCSFileUploadViewModel, ProcessXmlFile>(model);
                validationId = await client.SendAsync(User.GetAccessToken(), request);
            }

            if (Request.IsAjaxRequest())
            {
                return Json(validationId);
            }
            else
            {
                return RedirectToAction("ViewErrorsAndWarnings", "MemberRegistration",
                    new { area = "Scheme", memberUploadId = validationId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewErrorsAndWarnings(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));

                var memberUpload = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadById(pcsId, memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    await SetBreadcrumb(pcsId, ManageMembersActivity);

                    return View("ViewErrorsAndWarnings",
                        new MemberUploadResultViewModel { ErrorData = errors, TotalCharges = memberUpload.TotalCharges });
                }

                return RedirectToAction("XmlHasNoErrors", new { pcsId, memberUploadId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> XmlHasNoErrors(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));

                var memberUpload =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadById(pcsId, memberUploadId));

                await SetBreadcrumb(pcsId, ManageMembersActivity);

                return View("XmlHasNoErrors",
                     new MemberUploadResultViewModel
                     {
                         ErrorData = errors,
                         TotalCharges = memberUpload.TotalCharges
                     });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> XmlHasNoErrors(Guid pcsId, Guid memberUploadId, MemberUploadResultViewModel viewModel)
        {
            using (var client = apiClient())
            {
                // TODO: insert request including check against submitting a member upload with errors or different PCS here...

                if (!ModelState.IsValid)
                {
                    var errors =
                    await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId));
                    viewModel.ErrorData = errors;
                    return View(viewModel);
                }

                await client.SendAsync(User.GetAccessToken(), new MemberUploadSubmission(pcsId, memberUploadId));

                return RedirectToAction("SuccessfulSubmission", new { pcsId, memberUploadId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> SuccessfulSubmission(Guid pcsId, Guid memberUploadId)
        {
            MemberUploadData memberUploadData;

            using (var client = apiClient())
            {
                memberUploadData = await client.SendAsync(
                    User.GetAccessToken(),
                    new GetMemberUploadById(pcsId, memberUploadId));
            }

            if (!memberUploadData.IsSubmitted)
            {
                throw new Exception("The specified member upload has not yet been submitted.");
            }

            var model = new SuccessfulSubmissionViewModel
            {
                PcsId = pcsId,
                MemberUploadId = memberUploadId,
                ComplianceYear = memberUploadData.ComplianceYear.Value
            };

            await SetBreadcrumb(pcsId, ManageMembersActivity);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadErrorsAndWarnings(Guid pcsId, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                IEnumerable<UploadErrorData> errors =
                    (await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(pcsId, memberUploadId)))
                    .OrderByDescending(e => e.ErrorLevel);

                var schemePublicInfo = await cache.FetchSchemePublicInfo(pcsId);

                CsvWriter<UploadErrorData> csvWriter = csvWriterFactory.Create<UploadErrorData>();
                csvWriter.DefineColumn("Type", e => (int)e.ErrorLevel >= 5 ? "Error" : "Warning");
                csvWriter.DefineColumn("Description", e => e.Description);

                string csv = csvWriter.Write(errors);

                var csvFilename = string.Format("{0}_memberregistration_errors_warnings_{1}.csv", schemePublicInfo.ApprovalNo, DateTime.Now.ToString("ddMMyyyy_HHmm"));

                byte[] fileContent = new UTF8Encoding().GetBytes(csv);
                return File(fileContent, "text/csv", CsvFilenameFormat.FormatFileName(csvFilename));
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetProducerCSV(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var producerCSVData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerCSV(pcsId, complianceYear));

                byte[] data = new UTF8Encoding().GetBytes(producerCSVData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerCSVData.FileName));
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName == "AuthorisationRequired")
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                object pcsIdObject;
                if (filterContext.ActionParameters.TryGetValue("pcsId", out pcsIdObject))
                {
                    SchemeStatus status;
                    Guid pcsId = (Guid)pcsIdObject;

                    using (var client = apiClient())
                    {
                        var schemeStatusTask = Task<SchemeStatus>.Run(() => client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId)));
                        schemeStatusTask.Wait();

                        status = schemeStatusTask.Result;
                    }

                    if (status != SchemeStatus.Approved)
                    {
                        filterContext.Result = RedirectToAction("AuthorisationRequired", new { pcsID = pcsId });
                    }
                    else
                    {
                        base.OnActionExecuting(filterContext);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The PCS ID could not be retrieved.");
                }
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.SchemeInfo = await cache.FetchSchemePublicInfo(organisationId);
        }
    }
}