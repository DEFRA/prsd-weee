namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Aatf.Mappings.ToViewModel;
    using Attributes;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Email;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Extensions;
    using Filters;
    using Prsd.Core.Extensions;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.Shared;

    public class ManageEvidenceNotesController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly ConfigurationService configurationService;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient, 
            ISessionService sessionService,
            IMvcTemplateExecutor templateExecutor, 
            IPdfDocumentProvider pdfDocumentProvider,
            ConfigurationService configurationService) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.sessionService = sessionService;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.configurationService = configurationService;
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> Index(Guid pcsId, 
            string tab = null,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null,
            int page = 1)
        {
            sessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferTonnageViewModelKey);
            sessionService.ClearTransferSessionObject(SessionKeyConstant.TransferNoteKey);

            using (var client = this.apiClient())
            {
                var scheme = await Cache.FetchSchemePublicInfo(pcsId);

                await SetBreadcrumb(pcsId);

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
                var selectedComplianceYear = SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel);

                if (tab == null) 
                { 
                    tab = DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.Summary); 
                }

                var value = tab.GetValueFromDisplayName<ManageEvidenceNotesDisplayOptions>();

                switch (value)
                {
                    case ManageEvidenceNotesDisplayOptions.Summary:
                        return await CreateAndPopulateEvidenceSummaryViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, selectedComplianceYear);
                    case ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);
                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);
                    case ManageEvidenceNotesDisplayOptions.OutgoingTransfers:
                        return await CreateAndPopulateOutgoingTransfersEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);
                    default:
                        return await CreateAndPopulateEvidenceSummaryViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, selectedComplianceYear);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(Guid organisationId, int complianceYear)
        {
            return RedirectToAction("TransferEvidenceNote", "TransferEvidence", new { pcsId = organisationId, complianceYear = complianceYear });
        }

        private async Task<ActionResult> CreateAndPopulateReviewSubmittedEvidenceViewModel(Guid organisationId,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber, int selectedComplianceYear)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(organisationId, 
                    new List<NoteStatus>() { NoteStatus.Submitted },
                    selectedComplianceYear, new List<NoteType>() { NoteType.Evidence, NoteType.Transfer }, false, pageNumber, int.MaxValue, null));

                var model = mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                    new SchemeTabViewModelMapTransfer(organisationId, result, scheme, currentDate, selectedComplianceYear, pageNumber, int.MaxValue));

                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>
                    (new ManageEvidenceNoteTransfer(organisationId, null, null, null, selectedComplianceYear, currentDate));

                return View("ReviewSubmittedEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid pcsId,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber, int selectedComplianceYear)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesByOrganisationRequest(pcsId, new List<NoteStatus>() 
                    { 
                        NoteStatus.Approved, 
                        NoteStatus.Rejected,
                        NoteStatus.Void,
                        NoteStatus.Returned
                    }, selectedComplianceYear, new List<NoteType>() { NoteType.Evidence, NoteType.Transfer }, false, pageNumber, 
                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize, null));

                var model = mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                 new SchemeTabViewModelMapTransfer(pcsId, result, scheme, currentDate, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, null, null, selectedComplianceYear, currentDate));

                return View("ViewAndTransferEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateOutgoingTransfersEvidenceViewModel(Guid pcsId,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber, int selectedComplianceYear)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesByOrganisationRequest(pcsId, new List<NoteStatus>()
                    {
                        NoteStatus.Draft,
                        NoteStatus.Approved,
                        NoteStatus.Rejected,
                        NoteStatus.Submitted,
                        NoteStatus.Void,
                        NoteStatus.Returned
                    }, selectedComplianceYear, new List<NoteType>() { NoteType.Transfer },
                    true, 
                    pageNumber, 
                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize,
                    manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue));

                var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                            new RecipientWasteStatusFilterBase(null,
                            null,
                            null,
                            manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                            null, 
                            null, 
                            false, true));

                var model = mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                      new SchemeTabViewModelMapTransfer(pcsId, result, scheme, currentDate, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, recipientWasteStatusViewModel, null, selectedComplianceYear, currentDate));

                return View("OutgoingTransfers", model);
            }
        }
        
        [HttpGet]
        [CheckCanApproveNote]
        [NoCacheFilter]
        public async Task<ActionResult> ReviewEvidenceNote(Guid pcsId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId);

                ReviewEvidenceNoteViewModel model = await GetNote(pcsId, evidenceNoteId, client);

                if (model.ViewEvidenceNoteViewModel.Status != NoteStatus.Submitted)
                {
                    return RedirectToAction("Index", "ManageEvidenceNotes", new { pcsId, @tab = ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString() });
                }

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReviewEvidenceNote(ReviewEvidenceNoteViewModel model)
        {
            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var status = model.SelectedEnumValue;

                    var request = new SetNoteStatusRequest(model.ViewEvidenceNoteViewModel.Id, status, model.Reason);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = (NoteUpdatedStatusEnum)request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("ViewEvidenceNote", 
                        new { organisationId = model.OrganisationId, evidenceNoteId = request.NoteId, selectedComplianceYear = model.ViewEvidenceNoteViewModel.ComplianceYear });
                }

                await SetBreadcrumb(model.OrganisationId);

                model = await GetNote(model.ViewEvidenceNoteViewModel.SchemeId, model.ViewEvidenceNoteViewModel.Id, client);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> ViewEvidenceNote(Guid pcsId, Guid evidenceNoteId, string redirectTab = null, int page = 1, bool openedInNewTab = false)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus], false)
                {
                    SchemeId = pcsId,
                    RedirectTab = redirectTab,
                    OpenedInNewTab = openedInNewTab
                });

                ViewBag.Page = page;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndPopulateEvidenceSummaryViewModel(Guid pcsId, SchemePublicInfo scheme, DateTime currentDate, 
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, int selectedComplianceYear)
        {
            using (var client = apiClient())
            {
                GetObligationSummaryRequest request = null;
                if (scheme.IsBalancingScheme)
                {
                    // PBS do not have a scheme id - we send a null to the Stored Proc which will use organisation id instead
                    request = new GetObligationSummaryRequest(null, pcsId, selectedComplianceYear);

                    var evidenceSummaryData = await client.SendAsync(User.GetAccessToken(), request);

                    var pbsSummaryModel = mapper.Map<SummaryEvidenceViewModel>
                        (new ViewEvidenceSummaryViewModelMapTransfer(pcsId, evidenceSummaryData, manageEvidenceNoteViewModel, scheme, currentDate, selectedComplianceYear));

                    pbsSummaryModel.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, null, null, selectedComplianceYear, currentDate));

                    return View("SummaryEvidencePBS", pbsSummaryModel);
                }

                // used by Scheme users
                request = new GetObligationSummaryRequest(scheme.SchemeId, pcsId, selectedComplianceYear);

                var obligationEvidenceSummaryData = await client.SendAsync(User.GetAccessToken(), request);

                var summaryModel = mapper.Map<SummaryEvidenceViewModel>
                    (new ViewEvidenceSummaryViewModelMapTransfer(pcsId, obligationEvidenceSummaryData, manageEvidenceNoteViewModel, scheme, currentDate, selectedComplianceYear));

                summaryModel.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, null, null, selectedComplianceYear, currentDate));

                return View("SummaryEvidence", summaryModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNotePdf(Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null, true) { PrintableVersion = true });

                var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadEvidenceNotePdf", model);

                var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

                var timestamp = SystemTime.Now;
                var fileName = $"{result.ComplianceYear}_{model.ReferenceDisplay}{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.pdf";

                return File(pdf, "application/pdf", fileName);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNotesReport(Guid pcsId, int complianceYear, TonnageToDisplayReportEnum tonnageToDisplay)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteReportRequest(pcsId, null, tonnageToDisplay, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceTransfersReport(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetTransferNoteReportRequest(complianceYear, pcsId);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceSummaryReport(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(null, null, pcsId, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        private int SelectedComplianceYear(DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            return ComplianceYearHelper.GetSelectedComplianceYear(manageEvidenceNoteViewModel, currentDate);
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid pcsId, Guid evidenceNoteId, IWeeeClient client)
        {
            var result = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNoteForSchemeRequest(evidenceNoteId));

            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null, false)
            {
                SchemeId = pcsId
            });

            return model;
        }
    }
}