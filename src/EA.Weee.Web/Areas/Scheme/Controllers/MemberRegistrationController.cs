namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.Shared;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;

    public class MemberRegistrationController : ExternalSiteController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IFileConverterService fileConverter;
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public MemberRegistrationController(
            Func<IWeeeClient> apiClient,
            IFileConverterService fileConverter,
            IWeeeCache cache,
            BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.fileConverter = fileConverter;
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        [HttpGet]
        public async Task<ActionResult> AuthorizationRequired(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var status = await client.SendAsync(User.GetAccessToken(), new GetSchemeStatus(pcsId));

                if (status == SchemeStatus.Approved)
                {
                    return RedirectToAction("Summary", "MemberRegistration");
                }

                await SetBreadcrumb(pcsId, "Manage scheme");
                return View(new AuthorizationRequiredViewModel
                {
                    Status = status
                });
            }
        }

        [HttpGet]
        public async Task<ViewResult> AddOrAmendMembers(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(pcsId));
                if (orgExists)
                {
                    await SetBreadcrumb(pcsId, "Manage scheme");
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
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(500);
                }
                else
                {
                    await SetBreadcrumb(pcsId, "Manage scheme");
                    return View(model);
                }
            }

            var fileData = fileConverter.Convert(model.File);

            Guid validationId;
                
            using (var client = apiClient())
            {
                validationId = await client.SendAsync(User.GetAccessToken(), new ProcessXMLFile(pcsId, fileData));
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
        public async Task<ActionResult> Summary(Guid pcsId)
        {
            using (var client = apiClient())
            {
                List<int> years = await client.SendAsync(User.GetAccessToken(), new GetComplianceYears(pcsId));

                if (years.Count > 0)
                {
                    await SetBreadcrumb(pcsId, "Manage scheme");
                    return View(years);
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

                var memberUpload = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadById(pcsId, memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    await SetBreadcrumb(pcsId, "Manage scheme");
                    return View("ViewErrorsAndWarnings",
                        new MemberUploadResultViewModel { MemberUploadId = memberUploadId, ErrorData = errors, TotalCharges = memberUpload.TotalCharges });
                }

                await SetBreadcrumb(pcsId, "Manage scheme");
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

                await client.SendAsync(User.GetAccessToken(), new MemberUploadSubmission(pcsId, viewModel.MemberUploadId));

                return RedirectToAction("SuccessfulSubmission", new { pcsId = pcsId, memberUploadId = viewModel.MemberUploadId });
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

            await SetBreadcrumb(pcsId, "Manage scheme");
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetProducerCSV(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var producerCSVData = await client.SendAsync(User.GetAccessToken(),
                    new GetProducerCSV(pcsId, complianceYear));

                byte[] data = new UTF8Encoding().GetBytes(producerCSVData.FileContent);

                return File(data, "text/csv", producerCSVData.FileName);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName == "AuthorizationRequired")
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
                        filterContext.Result = RedirectToAction("AuthorizationRequired", new { pcsID = pcsId });
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
        }
    }
}