namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Authorization;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using Weee.Requests.Scheme;

    public class OutgoingTransfersController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;

        public OutgoingTransfersController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient, 
            ISessionService sessionService) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult> EditTonnages(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var request = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.TransferNoteKey);

                var existingEvidenceNoteIds = noteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

                if (request != null)
                {
                    existingEvidenceNoteIds = existingEvidenceNoteIds.Union(request.EvidenceNoteIds).Distinct().ToList();
                    existingEvidenceNoteIds.RemoveAll(a => !request.EvidenceNoteIds.Contains(a));
                }

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, noteData.CategoryIds, existingEvidenceNoteIds));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, noteData, pcsId);

                var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

                return this.View("EditTonnages", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditDraftTransfer(Guid pcsId, Guid evidenceNoteId, int? selectedComplianceYear, bool? returnToView)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId,
                    noteData, null)
                {
                    SelectedComplianceYear = selectedComplianceYear,
                    Edit = true,
                    ReturnToView = returnToView
                });

                return this.View("EditDraftTransfer", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SubmittedTransfer(Guid pcsId, Guid evidenceNoteId, int? selectedComplianceYear, bool? returnToView)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var result = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                ReviewTransferNoteViewModel model = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId, result, null)
                {
                    SchemeId = pcsId,
                    SelectedComplianceYear = selectedComplianceYear
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
                        SchemeId = model.OrganisationId,
                        SelectedComplianceYear = model.ViewTransferNoteViewModel.ComplianceYear
                    });

                    return View("DownloadTransferNote", modelRefreshed);
                }

                TransferEvidenceNoteData noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(model.ViewTransferNoteViewModel.EvidenceNoteId));

                var refreshedModel = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(model.ViewTransferNoteViewModel.SchemeId, noteData, null)
                {
                    SchemeId = model.ViewTransferNoteViewModel.SchemeId,
                    SelectedComplianceYear = model.ViewTransferNoteViewModel.SelectedComplianceYear.Value
                });

                return View("SubmittedTransfer", refreshedModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditTransferFrom(Guid pcsId, Guid evidenceNoteId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, noteData.CategoryIds));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, noteData, pcsId);

                var model =
                    mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

                return this.View("EditTransferFrom", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransferFrom(TransferEvidenceNotesViewModel model)
        {
            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    //TODO: Remove this API call when selected categories are passed from the select category edit when it is in place
                    var noteData = await client.SendAsync(User.GetAccessToken(),
                        new GetTransferEvidenceNoteForSchemeRequest(model.ViewTransferNoteViewModel.EvidenceNoteId));

                    var selectedEvidenceNotes =
                        model.SelectedEvidenceNotePairs.Where(a => a.Value).Select(b => b.Key);

                    var updatedTransferRequest =
                        new TransferEvidenceNoteRequest(model.PcsId, model.RecipientId, noteData.CategoryIds,
                            selectedEvidenceNotes.ToList());

                    sessionService.SetTransferSessionObject(Session, updatedTransferRequest,
                        SessionKeyConstant.TransferNoteKey);

                    return RedirectToRoute("Scheme_edit_transfer_tonnages",
                        new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
                }
            }

            return this.View("EditTransferFrom", model);
        }
    }
}