namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Attributes;
    using Core.Helpers;
    using Requests;
    using ViewModels;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class OutgoingTransfersController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly ITransferEvidenceRequestCreator transferEvidenceRequestCreator;
        
        public OutgoingTransfersController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient, 
            ISessionService sessionService,
            ITransferEvidenceRequestCreator transferEvidenceRequestCreator) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.sessionService = sessionService;
            this.transferEvidenceRequestCreator = transferEvidenceRequestCreator;
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        public async Task<ActionResult> EditTonnages(Guid pcsId, Guid evidenceNoteId, bool? returnToEditDraftTransfer)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var model = await TransferEvidenceTonnageViewModel(pcsId, evidenceNoteId, client, returnToEditDraftTransfer);

                return this.View("EditTonnages", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTonnages(TransferEvidenceTonnageViewModel model)
        {
            await SetBreadcrumb(model.PcsId);

            if (model.Action == ActionEnum.Back)
            {
                sessionService.SetTransferSessionObject(Session, model, SessionKeyConstant.EditTransferEvidenceTonnageViewModel);

                if (model.ReturnToEditDraftTransfer.Value)
                {
                    return RedirectToAction("EditDraftTransfer", "OutgoingTransfers", new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId, returnToView = false });
                }
                else
                {
                    return RedirectToAction("EditTransferFrom", "OutgoingTransfers", new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
                }
            }

            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.OutgoingTransferKey);

                    var updatedRequest = transferEvidenceRequestCreator.EditSelectTonnageToRequest(transferRequest, model);

                    var route = model.Action == ActionEnum.Submit
                        ? SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName :
                            SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName;

                    await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    NoteUpdatedStatusEnum updateStatus;
                    if (model.ViewTransferNoteViewModel.Status == NoteStatus.Returned)
                    {
                        switch (model.Action)
                        {
                            case ActionEnum.Save:
                                updateStatus = NoteUpdatedStatusEnum.ReturnedSaved;
                                break;
                            case ActionEnum.Submit:
                                updateStatus = NoteUpdatedStatusEnum.ReturnedSubmitted;
                                break;
                            default:
                                updateStatus = (NoteUpdatedStatusEnum)updatedRequest.Status;
                                break;
                        }
                    }
                    else
                    {
                        updateStatus = (NoteUpdatedStatusEnum)updatedRequest.Status;
                    }

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = updateStatus;

                    return new RedirectToRouteResult(route, new RouteValueDictionary()
                    {
                        { "pcsId", model.PcsId },
                        { "evidenceNoteId", model.ViewTransferNoteViewModel.EvidenceNoteId },
                        {
                            "redirectTab",
                            Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers)
                        }
                    });
                }

                var updatedModel = await TransferEvidenceTonnageViewModel(model.PcsId, model.ViewTransferNoteViewModel.EvidenceNoteId, client, false);

                return this.View("EditTonnages", updatedModel);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        public async Task<ActionResult> EditDraftTransfer(Guid pcsId, Guid evidenceNoteId, bool? returnToView, string redirectTab = null)
        {
            await SetBreadcrumb(pcsId);

            ClearSessionValues();

            redirectTab = redirectTab ?? ManageEvidenceNotesDisplayOptions.OutgoingTransfers.ToDisplayString();

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId,
                    noteData, null)
                {
                    Edit = true,
                    ReturnToView = returnToView,
                    RedirectTab = redirectTab,
                    SystemDateTime = currentDate
                });

                return this.View("EditDraftTransfer", model);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        public async Task<ActionResult> SubmittedTransfer(Guid pcsId, Guid evidenceNoteId, bool? returnToView, string redirectTab)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                if (noteData.Status != NoteStatus.Submitted)
                {
                    return RedirectToAction("Index", "ManageEvidenceNotes", new { pcsId, @tab = redirectTab });
                }

                var model = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId, noteData, null)
                {
                    OrganisationId = pcsId,
                    ReturnToView = returnToView,
                    RedirectTab = redirectTab,
                });

                return this.View("SubmittedTransfer", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmittedTransfer(ReviewTransferNoteViewModel model)
        {
            await SetBreadcrumb(model.OrganisationId);

            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var status = model.SelectedEnumValue;

                    var request = new SetNoteStatusRequest(model.ViewTransferNoteViewModel.EvidenceNoteId, status, model.Reason);

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = (NoteUpdatedStatusEnum)request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    var requestRefreshed = new GetTransferEvidenceNoteForSchemeRequest(request.NoteId);

                    TransferEvidenceNoteData note = await client.SendAsync(User.GetAccessToken(), requestRefreshed);

                    var modelRefreshed = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(model.OrganisationId, note, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification])
                    {
                        OrganisationId = model.OrganisationId
                    });

                    return View("DownloadTransferNote", modelRefreshed);
                }

                TransferEvidenceNoteData noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(model.ViewTransferNoteViewModel.EvidenceNoteId));

                var refreshedModel = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(model.ViewTransferNoteViewModel.SchemeId, noteData, null)
                {
                    OrganisationId = model.ViewTransferNoteViewModel.SchemeId
                });

                return View("SubmittedTransfer", refreshedModel);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        public async Task<ActionResult> EditTransferFrom(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.OutgoingTransferKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId);
                }

                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, noteData.ComplianceYear));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, transferRequest, noteData, pcsId);

                var evidenceNoteIds = transferRequest.EvidenceNoteIds;
                if (evidenceNoteIds != null)
                {
                    mapperObject.SessionEvidenceNotesId = evidenceNoteIds;
                }

                var excludeEvidenceNoteIds = transferRequest.ExcludeEvidenceNoteIds;
                if (excludeEvidenceNoteIds != null)
                {
                    mapperObject.ExcludeEvidenceNoteIds = excludeEvidenceNoteIds;
                }

                var model =
                    mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

                return this.View("EditTransferFrom", model);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        public async Task<ActionResult> EditCategories(Guid pcsId, Guid evidenceNoteId, int page = 1)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var model = await TransferEvidenceNoteCategoriesViewModel(pcsId, evidenceNoteId, client, null);

                MananageCategoryIdsInSession(model);

                ViewBag.Page = page;

                return this.View("EditCategories", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategories(TransferEvidenceNoteCategoriesViewModel model)
        {
            await SetBreadcrumb(model.PcsId);

            if (ModelState.IsValid)
            {
                var existingRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.OutgoingTransferKey);

                var transferRequest = transferEvidenceRequestCreator.SelectCategoriesToRequest(model, existingRequest);

                sessionService.SetTransferSessionObject(Session, transferRequest, SessionKeyConstant.OutgoingTransferKey);

                return RedirectToRoute("Scheme_edit_transfer_notes",
                    new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
            }

            using (var client = apiClient())
            {
                var refreshedModel = await TransferEvidenceNoteCategoriesViewModel(model.PcsId, model.ViewTransferNoteViewModel.EvidenceNoteId, client, model);

                return this.View("EditCategories", refreshedModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransferFrom(TransferEvidenceNotesViewModel model)
        {
            await SetBreadcrumb(model.PcsId);

            if (model.Action == ActionEnum.Back)
            {
                var outgoingTransfer = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                  SessionKeyConstant.OutgoingTransferKey);

                if (outgoingTransfer == null)
                {
                    return RedirectToManageEvidence(model.PcsId);
                }

                var selectedEvidenceNotes =
                   model.SelectedEvidenceNotePairs.Where(a => a.Value == true).Select(b => b.Key);

                var unselectedEvidenceNotes = model.SelectedEvidenceNotePairs.Where(a => a.Value == false).Select(b => b.Key);

                var updatedTransferRequest =
                    new TransferEvidenceNoteRequest(model.PcsId, model.RecipientId, outgoingTransfer.CategoryIds,
                        selectedEvidenceNotes.ToList(), unselectedEvidenceNotes.ToList());

                sessionService.SetTransferSessionObject(Session, updatedTransferRequest,
                    SessionKeyConstant.OutgoingTransferKey);

                return RedirectToAction("EditCategories", "OutgoingTransfers", new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId});
            }

            if (ModelState.IsValid)
            {
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.OutgoingTransferKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(model.PcsId);
                }

                var selectedEvidenceNotes =
                    model.SelectedEvidenceNotePairs.Where(a => a.Value).Select(b => b.Key);

                var updatedTransferRequest =
                    new TransferEvidenceNoteRequest(model.PcsId, model.RecipientId, transferRequest.CategoryIds,
                        selectedEvidenceNotes.ToList());

                sessionService.SetTransferSessionObject(Session, updatedTransferRequest,
                    SessionKeyConstant.OutgoingTransferKey);

                return RedirectToRoute("Scheme_edit_transfer_tonnages",
                    new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId, returnToEditDraftTransfer = false });
            }

            return this.View("EditTransferFrom", model);
        }

        private async Task<TransferEvidenceNoteCategoriesViewModel> TransferEvidenceNoteCategoriesViewModel(Guid pcsId, 
            Guid evidenceNoteId, 
            IWeeeClient client,
            TransferEvidenceNoteCategoriesViewModel existingModel)
        {
            var noteData =
                await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

            var schemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(false));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(noteData, schemes, pcsId, existingModel);

            var model =
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNoteCategoriesViewModel>(mapperObject);
            return model;
        }

        private async Task<TransferEvidenceTonnageViewModel> TransferEvidenceTonnageViewModel(Guid pcsId, Guid evidenceNoteId, IWeeeClient client, bool? returnToEditDraftTransfer)
        {
            var noteData =
                await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

            var request =
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.OutgoingTransferKey);

            var existingModel = sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(Session, SessionKeyConstant.EditTransferEvidenceTonnageViewModel);
            sessionService.ClearTransferSessionObject(Session, SessionKeyConstant.EditTransferEvidenceTonnageViewModel);

            var existingEvidenceNoteIds = noteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

            if (request?.EvidenceNoteIds != null)
            {
                existingEvidenceNoteIds = existingEvidenceNoteIds.Union(request.EvidenceNoteIds).Distinct().ToList();
                existingEvidenceNoteIds.RemoveAll(a => !request.EvidenceNoteIds.Contains(a));
            }

            var noteIds = request != null ? request.CategoryIds : noteData.CategoryIds;

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, noteIds, noteData.ComplianceYear, existingEvidenceNoteIds));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, request, noteData, pcsId)
            {
                ExistingTransferTonnageViewModel = existingModel,
                ReturnToEditDraftTransfer = returnToEditDraftTransfer
            };

            var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

            return model;
        }

        private ActionResult RedirectToManageEvidence(Guid pcsId)
        {
            return RedirectToAction("Index", "ManageEvidenceNotes",
                new { pcsId, area = "Scheme", tab = ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString() });
        }

        private void MananageCategoryIdsInSession(TransferEvidenceNoteCategoriesViewModel model)
        {
            var existingRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.OutgoingTransferKey);

            if (existingRequest != null)
            {
                var alreadyExistingCategoryIds = model.CategoryValues.Select(c => c.CategoryId).ToList();
                model.CategoryBooleanViewModels.Where(c => alreadyExistingCategoryIds.Contains(c.CategoryId)).ToList()
                   .ForEach(c => c.Selected = false);

                var selectedCategoryIds = existingRequest.CategoryIds;
                model.CategoryBooleanViewModels.Where(c => selectedCategoryIds.Contains(c.CategoryId)).ToList()
                    .ForEach(c => c.Selected = true);

                model.SelectedSchema = existingRequest.RecipientId;
            }
        }

        private void ClearSessionValues()
        {
            sessionService.ClearTransferSessionObject(Session, SessionKeyConstant.OutgoingTransferKey);
            sessionService.ClearTransferSessionObject(Session, SessionKeyConstant.EditTransferEvidenceTonnageViewModel);
        }
    }
}