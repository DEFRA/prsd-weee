namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Constant;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using Infrastructure;
    using Mappings.ToViewModels;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.ManageEvidenceNotes;
    using Weee.Requests.AatfEvidence;

    public class TransferEvidenceController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ITransferEvidenceRequestCreator transferNoteRequestCreator;
        private readonly ISessionService sessionService;

        public TransferEvidenceController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper, ITransferEvidenceRequestCreator transferNoteRequestCreator, IWeeeCache cache, ISessionService sessionService) : base(breadcrumb, cache)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
            this.transferNoteRequestCreator = transferNoteRequestCreator;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult> TransferEvidenceNote(Guid pcsId)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            var model = new TransferEvidenceNoteCategoriesViewModel
            {
                OrganisationId = pcsId,
                SchemasToDisplay = await GetApprovedSchemes(pcsId)
            };

            return this.View("TransferEvidenceNote", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteCategoriesViewModel model)
        {
            var selectedCategoryIds = model.SelectedCategoryValues;

            if (ModelState.IsValid)
            {
                var transferRequest = transferNoteRequestCreator.SelectCategoriesToRequest(model);

                sessionService.SetTransferSessionObject(Session, transferRequest, SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("TransferFrom", "TransferEvidence", new { area = "Scheme", pcsId = model.OrganisationId });
            }

            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

            model.AddCategoryValues();
            CheckedCategoryIds(model, selectedCategoryIds);
            model.SchemasToDisplay = await GetApprovedSchemes(model.OrganisationId);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> TransferFrom(Guid pcsId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);
                
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId);
                }

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, transferRequest, pcsId);

                var model =
                    mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

                return this.View("TransferFrom", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferFrom(TransferEvidenceNotesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var transferRequest =
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                        SessionKeyConstant.TransferNoteKey);

                var selectedEvidenceNotes =
                    model.SelectedEvidenceNotePairs.Where(a => a.Value.Equals(true)).Select(b => b.Key);

                var updatedTransferRequest =
                    new TransferEvidenceNoteRequest(model.PcsId, transferRequest.SchemeId, transferRequest.CategoryIds, selectedEvidenceNotes.ToList());

                sessionService.SetTransferSessionObject(Session, updatedTransferRequest, SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("TransferTonnage", "TransferEvidence", new { area = "Scheme", pcsId = model.PcsId, transferAllTonnage = false });
            }

            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            return View("TransferFrom", model);
        }

        [HttpGet]
        public async Task<ActionResult> TransferTonnage(Guid pcsId, bool transferAllTonnage = false)
        {
            using (var client = this.apiClient())
            {
                var model = await TransferEvidenceTonnageViewModel(pcsId, transferAllTonnage, client);

                if (model == null)
                {
                    return RedirectToManageEvidence(pcsId);
                }

                return this.View("TransferTonnage", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferTonnage(TransferEvidenceTonnageViewModel model)
        {
            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest =
                        sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                            SessionKeyConstant.TransferNoteKey);

                    var updatedRequest = transferNoteRequestCreator.SelectTonnageToRequest(transferRequest, model);

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = true;

                    var id = await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    return RedirectToAction("TransferredEvidence", "TransferEvidence",
                        new { pcsId = model.PcsId, evidenceNoteId = id });
                }

                var updatedModel = await TransferEvidenceTonnageViewModel(model.PcsId, false, client);

                return this.View("TransferTonnage", updatedModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> TransferredEvidence(Guid pcsId, Guid evidenceNoteId, int? selectedComplianceYear, string redirectTab)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(),
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(pcsId,
                    noteData, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification])
                {
                    SelectedComplianceYear = selectedComplianceYear,
                    RedirectTab = redirectTab
                });

                return this.View("TransferredEvidence", model);
            }
        }

        private async Task<TransferEvidenceTonnageViewModel> TransferEvidenceTonnageViewModel(Guid pcsId, bool transferAllTonnage, IWeeeClient client)
        {
            await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

            var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                SessionKeyConstant.TransferNoteKey);

            if (transferRequest == null)
            {
                return null;
            }

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, transferRequest.EvidenceNoteIds));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, transferRequest, pcsId)
            {
                TransferAllTonnage = transferAllTonnage
            };

            var model = mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

            return model;
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

                organisationSchemes.RemoveAll(s => s.OrganisationId.Equals(pcsId));

                return organisationSchemes;
            }
        }

        private ActionResult RedirectToManageEvidence(Guid pcsId)
        {
            return RedirectToAction("Index", "ManageEvidenceNotes",
                new { pcsId, area = "Scheme", tab = ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString() });
        }
    }
}