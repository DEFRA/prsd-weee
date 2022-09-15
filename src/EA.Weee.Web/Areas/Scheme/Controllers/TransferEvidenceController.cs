namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Api.Client;
    using Attributes;
    using Constant;
    using Core.AatfEvidence;
    using Core.Constants;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using Filters;
    using Infrastructure;
    using Infrastructure.PDF;
    using Mappings.ToViewModels;
    using Requests;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using ViewModels.ManageEvidenceNotes;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Shared;

    public class TransferEvidenceController : TransferEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ITransferEvidenceRequestCreator transferNoteRequestCreator;
        private readonly ConfigurationService configurationService;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly IMvcTemplateExecutor templateExecutor;

        public TransferEvidenceController(Func<IWeeeClient> apiClient,
            BreadcrumbService breadcrumb,
            IMapper mapper,
            ITransferEvidenceRequestCreator transferNoteRequestCreator,
            IWeeeCache cache,
            ISessionService sessionService,
            ConfigurationService configurationService,
            IPdfDocumentProvider pdfDocumentProvider,
            IMvcTemplateExecutor templateExecutor) : base(breadcrumb, cache, sessionService)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
            this.transferNoteRequestCreator = transferNoteRequestCreator;
            this.configurationService = configurationService;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.templateExecutor = templateExecutor;
        }

        [HttpGet]
        [CheckCanCreateTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> TransferEvidenceNote(Guid pcsId, int complianceYear)
        {
            await SetBreadcrumb(pcsId);

            var model = new TransferEvidenceNoteCategoriesViewModel
            {
                OrganisationId = pcsId,
                SchemasToDisplay = await GetApprovedSchemes(pcsId),
                ComplianceYear = complianceYear
            };

            var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                SessionKeyConstant.TransferNoteKey);

            if (transferRequest != null)
            {
                var categoryIds = transferRequest.CategoryIds;
                model.CategoryBooleanViewModels.Where(c => categoryIds.Contains(c.CategoryId)).ToList()
                    .ForEach(c => c.Selected = true);
                model.SelectedSchema = transferRequest.RecipientId;
            }

            return View("TransferEvidenceNote", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteCategoriesViewModel model)
        {
            var selectedCategoryIds = model.SelectedCategoryValues;

            if (ModelState.IsValid)
            {
                var existingTransferRequest =
                    SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                        SessionKeyConstant.TransferNoteKey);

                var transferRequestWithSelectedCategories =
                    transferNoteRequestCreator.SelectCategoriesToRequest(model, existingTransferRequest);

                SessionService.SetTransferSessionObject(Session, transferRequestWithSelectedCategories,
                    SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("TransferFrom", "TransferEvidence",
                    new { area = "Scheme", pcsId = model.OrganisationId, complianceYear = model.ComplianceYear });
            }

            await SetBreadcrumb(model.OrganisationId);

            model.AddCategoryValues();
            CheckedCategoryIds(model, selectedCategoryIds);
            model.SchemasToDisplay = await GetApprovedSchemes(model.OrganisationId);

            return View(model);
        }

        [HttpGet]
        [CheckCanCreateTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> TransferFrom(Guid pcsId, int complianceYear, int page = 1)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(pcsId);

                var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId, complianceYear);
                }

                var model = await TransferFromViewModel(pcsId, complianceYear, client, page, transferRequest);

                return View("TransferFrom", model);
            }
        }

        private async Task<TransferEvidenceNotesViewModel> TransferFromViewModel(Guid pcsId, int complianceYear,
            IWeeeClient client, int pageNumber, TransferEvidenceNoteRequest transferRequest)
        {
            var currentSelectedNotes = new EvidenceNoteSearchDataResult();
            if (transferRequest.EvidenceNoteIds.Any())
            {
                currentSelectedNotes = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesSelectedForTransferRequest(pcsId, transferRequest.EvidenceNoteIds, transferRequest.CategoryIds));
            }

            var availableNotes = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, complianceYear, transferRequest.EvidenceNoteIds, null, pageNumber,
                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(complianceYear,
                currentSelectedNotes,
                availableNotes,
                transferRequest,
                pcsId, pageNumber,
                configurationService.CurrentConfiguration.DefaultExternalPagingPageSize);

            var model =
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferFrom(TransferEvidenceNotesViewModel model)
        {
            await SetBreadcrumb(model.PcsId);

            var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                SessionKeyConstant.TransferNoteKey);

            transferRequest.UpdateSelectedNotes(model.EvidenceNotesDataList.Select(e => e.Id).ToList());

            if (!ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    model = await TransferFromViewModel(model.PcsId, model.ComplianceYear, client, model.PageNumber, transferRequest);
                }

                return View("TransferFrom", model);
            }

            return RedirectToAction("TransferTonnage", "TransferEvidence",
                new
                {
                    area = "Scheme", pcsId = model.PcsId, complianceYear = model.ComplianceYear,
                    transferAllTonnage = false
                });
        }

        [HttpGet]
        [CheckCanCreateTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> TransferTonnage(Guid pcsId, int complianceYear, bool transferAllTonnage = false)
        {
            using (var client = apiClient())
            {
                var model = await TransferEvidenceTonnageViewModel(pcsId, transferAllTonnage, complianceYear, client);

                if (model == null)
                {
                    return RedirectToManageEvidence(pcsId, complianceYear);
                }

                return View("TransferTonnage", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferTonnage(TransferEvidenceTonnageViewModel model)
        {
            if (model.Action == ActionEnum.Back)
            {
                SessionService.SetTransferSessionObject(Session, model, SessionKeyConstant.EditTransferTonnageViewModelKey);

                return RedirectToAction("TransferFrom", "TransferEvidence", new { pcsId = model.PcsId, complianceYear = model.ComplianceYear });
            }

            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.TransferNoteKey);

                    var updatedRequest = transferNoteRequestCreator.SelectTonnageToRequest(transferRequest, model);

                    var id = await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    var updateStatus = (NoteUpdatedStatusEnum)updatedRequest.Status;

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = updateStatus;

                    return RedirectToAction("TransferredEvidence", "TransferEvidence",
                        new { pcsId = model.PcsId, evidenceNoteId = id, redirectTab = Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers) });
                }

                var updatedModel = await TransferEvidenceTonnageViewModel(model.PcsId, false, model.ComplianceYear, client);

                return View("TransferTonnage", updatedModel);
            }
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> TransferredEvidence(Guid pcsId, Guid evidenceNoteId, string redirectTab, int page = 1)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var currentDateTime = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId,
                    noteData, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification])
                {
                    RedirectTab = redirectTab,
                    SystemDateTime = currentDateTime
                });

                ViewBag.Page = page;

                return View("TransferredEvidence", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmittedTransferNote(Guid schemeId, Guid evidenceNoteId, NoteStatus status)
        {
            using (var client = apiClient())
            {
                NoteUpdatedStatusEnum updateStatus;
                switch (status)
                {
                    case NoteStatus.Draft:
                        updateStatus = NoteUpdatedStatusEnum.Submitted;
                        break;
                    case NoteStatus.Returned:
                        updateStatus = NoteUpdatedStatusEnum.ReturnedSubmitted;
                        break;
                    default:
                        throw new InvalidOperationException("status is not valid");
                }

                SetNoteStatusRequest request = new SetNoteStatusRequest(evidenceNoteId, NoteStatus.Submitted);

                TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = updateStatus;

                string token = User.GetAccessToken();
                await client.SendAsync(token, request);

                return RedirectToRoute(SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName, new
                {
                    pcsId = schemeId, 
                    evidenceNoteId, 
                    redirectTab = Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers)
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadTransferEvidenceNote(Guid pcsId, Guid transferEvidenceNoteId)
        {
            using (var client = apiClient())
            {
                var request = new GetTransferEvidenceNoteForSchemeRequest(transferEvidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId, result, null));

                var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadTransferEvidenceNote", model);

                var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

                var timestamp = SystemTime.Now;
                var fileName = $"{model.ReferenceDisplay}{timestamp.ToString(DateTimeConstants.FilenameTimestampFormat)}.pdf";

                return File(pdf, "application/pdf", fileName);
            }
        }

        private async Task<TransferEvidenceTonnageViewModel> TransferEvidenceTonnageViewModel(Guid pcsId, bool transferAllTonnage, int complianceYear, IWeeeClient client)
        {
            await SetBreadcrumb(pcsId);

            var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                SessionKeyConstant.TransferNoteKey);

            if (transferRequest == null)
            {
                return null;
            }

            var existingModel = SessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(Session, SessionKeyConstant.EditTransferTonnageViewModelKey);
            SessionService.ClearTransferSessionObject(Session, SessionKeyConstant.EditTransferTonnageViewModelKey);

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesSelectedForTransferRequest(pcsId, transferRequest.EvidenceNoteIds, transferRequest.CategoryIds));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(complianceYear, result, transferRequest, pcsId)
            {
                TransferAllTonnage = transferAllTonnage,
                ExistingTransferTonnageViewModel = existingModel
            };

            var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<ActionResult> SelectEvidenceNote(TransferSelectEvidenceNoteModel model)
        {
            SelectEvidenceNote(model.SelectedEvidenceNoteId, SessionKeyConstant.TransferNoteKey);
         
            if (ModelState.IsValid)
            {
                return RedirectToAction("TransferFrom", new { pcsId = model.PcsId, model.ComplianceYear, model.Page });
            }

            using (var client = apiClient())
            {
                var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                var newModel = await TransferFromViewModel(model.PcsId, model.ComplianceYear, client, model.Page, transferRequest);

                return View("TransferFrom", newModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult DeselectEvidenceNote(TransferDeselectEvidenceNoteModel model)
        {
            DeselectEvidenceNote(model.SelectedEvidenceNoteId, SessionKeyConstant.TransferNoteKey);

            return RedirectToAction("TransferFrom", new { pcsId = model.PcsId, model.ComplianceYear, model.Page });
        }

        private void CheckedCategoryIds(TransferEvidenceNoteCategoriesViewModel model, List<int> ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    for (int i = 0; i < model.CategoryBooleanViewModels.Count; i++)
                    {
                        if (model.CategoryBooleanViewModels[i].CategoryId == id)
                        {
                            model.CategoryBooleanViewModels[i].Selected = true;
                        }
                    }
                }
            }
        }

        private async Task<List<OrganisationSchemeData>> GetApprovedSchemes(Guid pcsId)
        {
            using (var client = apiClient())
            {
                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(false));

                organisationSchemes.RemoveAll(s => s.Id.Equals(pcsId));

                return organisationSchemes;
            }
        }

        private ActionResult RedirectToManageEvidence(Guid pcsId, int complianceYear)
        {
            return RedirectToAction("Index", "ManageEvidenceNotes", new 
            { 
                pcsId, 
                area = "Scheme", 
                tab = Extensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence),
                selectedComplianceYear = complianceYear
            });
        }
    }
}