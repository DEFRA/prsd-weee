namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
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

                existingEvidenceNoteIds = existingEvidenceNoteIds.Union(request.EvidenceNoteIds).Distinct().ToList();

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
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var model = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId,
                    noteData, null));

                return this.View("SubmittedTransfer", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmittedTransfer(ReviewTransferNoteViewModel model)
        {
            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

            if (ModelState.IsValid)
            {
                //TODO: set note status request
            }
            
            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(model.ViewTransferNoteViewModel.EvidenceNoteId));

                var refreshedModel = mapper.Map<ReviewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(model.OrganisationId,
                    noteData, null));

                return this.View("SubmittedTransfer", refreshedModel);
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
                        model.SelectedEvidenceNotePairs.Where(a => a.Value.Equals(true)).Select(b => b.Key);

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