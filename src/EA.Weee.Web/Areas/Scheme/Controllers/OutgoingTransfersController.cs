namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
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
    using Filters;
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
        [NoCacheFilter]
        public async Task<ActionResult> EditTonnages(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var model = await TransferEvidenceTonnageViewModel(pcsId, evidenceNoteId, client);

                return this.View("EditTonnages", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTonnages(TransferEvidenceTonnageViewModel model)
        {
            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.TransferNoteKey);

                    var updatedRequest = transferEvidenceRequestCreator.EditSelectTonnageToRequest(transferRequest, model);

                    var route = model.Action == ActionEnum.Submit
                        ? SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName :
                            SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName;

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = true;

                    await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    return new RedirectToRouteResult(route, new RouteValueDictionary()
                    {
                        { "pcsId", model.PcsId },
                        { "evidenceNoteId", model.ViewTransferNoteViewModel.EvidenceNoteId },
                        {
                            "redirectTab",
                            Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions
                                .OutgoingTransfers)
                        }
                    });
                }

                var updatedModel = await TransferEvidenceTonnageViewModel(model.PcsId, model.ViewTransferNoteViewModel.EvidenceNoteId, client);

                return this.View("EditTonnages", updatedModel);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> EditDraftTransfer(Guid pcsId, Guid evidenceNoteId, bool? returnToView, string redirectTab = null)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

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
        [NoCacheFilter]
        public async Task<ActionResult> SubmittedTransfer(Guid pcsId, Guid evidenceNoteId, int? selectedComplianceYear, bool? returnToView, string redirectTab)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

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
            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var status = model.SelectedEnumValue;

                    var request = new SetNoteStatus(model.ViewTransferNoteViewModel.EvidenceNoteId, status, model.Reason);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = (NoteUpdatedStatusEnum)request.Status;
                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = true;

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
        [NoCacheFilter]
        public async Task<ActionResult> EditTransferFrom(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId);
                }

                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, noteData.ComplianceYear));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, transferRequest, noteData, pcsId);

                var model =
                    mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

                return this.View("EditTransferFrom", model);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> EditCategories(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var model = await TransferEvidenceNoteCategoriesViewModel(pcsId, evidenceNoteId, client, null);

                return this.View("EditCategories", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategories(TransferEvidenceNoteCategoriesViewModel model)
        {
            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            if (ModelState.IsValid)
            {
                var transferRequest = transferEvidenceRequestCreator.SelectCategoriesToRequest(model);

                sessionService.SetTransferSessionObject(Session, transferRequest, SessionKeyConstant.TransferNoteKey);

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
            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            if (ModelState.IsValid)
            {
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

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
                    SessionKeyConstant.TransferNoteKey);

                return RedirectToRoute("Scheme_edit_transfer_tonnages",
                    new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
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

        private async Task<TransferEvidenceTonnageViewModel> TransferEvidenceTonnageViewModel(Guid pcsId, Guid evidenceNoteId, IWeeeClient client)
        {
            var noteData =
                await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

            var request =
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.TransferNoteKey);

            var existingEvidenceNoteIds = noteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

            if (request?.EvidenceNoteIds != null)
            {
                existingEvidenceNoteIds = existingEvidenceNoteIds.Union(request.EvidenceNoteIds).Distinct().ToList();
                existingEvidenceNoteIds.RemoveAll(a => !request.EvidenceNoteIds.Contains(a));
            }

            var noteIds = request != null ? request.CategoryIds : noteData.CategoryIds;

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, noteIds, noteData.ComplianceYear, existingEvidenceNoteIds));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, request, noteData, pcsId);

            var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);
            return model;
        }

        private ActionResult RedirectToManageEvidence(Guid pcsId)
        {
            return RedirectToAction("Index", "ManageEvidenceNotes",
                new { pcsId, area = "Scheme", tab = ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString() });
        }
    }
}