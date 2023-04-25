﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Admin.Obligation;
    using Core.Scheme;
    using Extensions;
    using ViewModels.Obligations;
    using Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Filters;

    public class ObligationsController : ObligationsBaseController
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public ObligationsController(IAppConfiguration configuration, BreadcrumbService breadcrumb, IWeeeCache cache, Func<IWeeeClient> apiClient, IMapper mapper) : base(breadcrumb, cache)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
            this.mapper = mapper;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!configuration.EnablePCSObligations)
            {
                throw new InvalidOperationException("PCS Obligations is not enabled.");
            }

            Breadcrumb.InternalActivity = "Manage PCS obligations";

            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public ActionResult SelectAuthority()
        {
            return View("SelectAuthority", new SelectAuthorityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public ActionResult SelectAuthority(SelectAuthorityViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("UploadObligations", new { authority = model.SelectedAuthority.Value});
            }
            
            return View("SelectAuthority", model);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> UploadObligations(CompetentAuthority authority, Guid? id, int? selectedComplianceYear, bool displayNotification = false)
        {
            using (var client = apiClient())
            {
                List<SchemeObligationUploadErrorData> errorData = null;
                if (id.HasValue)
                {
                    errorData = await client.SendAsync(User.GetAccessToken(), new GetSchemeObligationUpload(id.Value));
                }

                var model = await UploadObligationsViewModel(authority, selectedComplianceYear, displayNotification, client, errorData);

                return View(model);
            }
        }

        private async Task<UploadObligationsViewModel> UploadObligationsViewModel(CompetentAuthority authority, int? selectedComplianceYear,
            bool displayNotification, IWeeeClient client, List<SchemeObligationUploadErrorData> errorData)
        {
            var complianceYears = await client.SendAsync(User.GetAccessToken(), new GetObligationComplianceYears(authority, true));
            var complianceYear = selectedComplianceYear ?? complianceYears.ElementAt(0);

            var schemeObligationData =
                await client.SendAsync(User.GetAccessToken(), new GetSchemeObligation(authority, complianceYear));

            var model = mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(
                new UploadObligationsViewModelMapTransfer()
                {
                    CompetentAuthority = authority,
                    ErrorData = errorData,
                    ObligationData = schemeObligationData,
                    SelectedComplianceYear = complianceYear,
                    ComplianceYears = complianceYears,
                    DisplayNotification = displayNotification
                });
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> UploadObligations(UploadObligationsViewModel model)
        {
            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var request = mapper.Map<UploadObligationsViewModel, SubmitSchemeObligation>(model);

                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("UploadObligations", new
                    {
                        authority = model.Authority,
                        id = result,
                        selectedComplianceYear = model.SelectedComplianceYear,
                        displayNotification = true
                    });
                }

                var refreshedModel = await UploadObligationsViewModel(model.Authority, model.SelectedComplianceYear,
                    false, client, null);

                if (ModelState.HasErrorForProperty<UploadObligationsViewModel, HttpPostedFileBase>(m => m.File))
                {
                    refreshedModel.DisplaySelectFileError = true;
                }

                return View(refreshedModel);
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> DownloadTemplate(CompetentAuthority authority)
        {
            using (var client = apiClient())
            {
                var fileData = await client.SendAsync(User.GetAccessToken(), new GetPcsObligationsCsv(authority));

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewObligationAndEvidenceSummary(int? selectedComplianceYear = null, Guid? schemeId = null)
        {
            using (var client = apiClient())
            {
                Breadcrumb.InternalActivity = "View PCS obligation and evidence summary";

                ObligationEvidenceSummaryData obligationEvidenceSummaryData = null;
                var schemeData = new List<SchemeData>();

                var complianceYears =
                    await client.SendAsync(User.GetAccessToken(), new GetObligationComplianceYears(null, true));

                var complianceYear = selectedComplianceYear ?? (complianceYears.Any() ? complianceYears.ElementAt(0) : (int?)null);

                if (complianceYear.HasValue)
                {
                    schemeData = await client.SendAsync(User.GetAccessToken(), new GetSchemesWithObligation(complianceYear.Value));

                    if (schemeId.HasValue)
                    {
                        var request = new GetObligationSummaryRequest(schemeId.Value, null, complianceYear.Value);
                        obligationEvidenceSummaryData = await client.SendAsync(User.GetAccessToken(), request);
                    }
                }

                var summaryModel = mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, obligationEvidenceSummaryData, complianceYears, schemeData));

                return View(summaryModel);
            }
        }
    }
}