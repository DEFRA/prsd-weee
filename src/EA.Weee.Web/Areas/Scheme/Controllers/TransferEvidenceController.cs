namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using Constant;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using Extensions;
    using Filters;
    using Infrastructure;
    using Mappings.ToViewModels;
    using Requests;
    using Services;
    using Services.Caching;
    using ViewModels;
    using ViewModels.ManageEvidenceNotes;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Shared;

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

            var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                  SessionKeyConstant.TransferNoteKey);

            if (transferRequest != null)
            {
                var categoryIds = transferRequest.CategoryIds;
                model.CategoryBooleanViewModels.Where(c => categoryIds.Contains(c.CategoryId)).ToList()
                    .ForEach(c => c.Selected = true);
                model.SelectedSchema = transferRequest.RecipientId;
            }

            return this.View("TransferEvidenceNote", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteCategoriesViewModel model)
        {
            var selectedCategoryIds = model.SelectedCategoryValues;

            if (ModelState.IsValid)
            {
                var existingTransferRequest =
                   sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                       SessionKeyConstant.TransferNoteKey);

                var transferRequestWithSelectedCategories = transferNoteRequestCreator.SelectCategoriesToRequest(model, existingTransferRequest);

                sessionService.SetTransferSessionObject(Session, transferRequestWithSelectedCategories, SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("TransferFrom", "TransferEvidence", new { area = "Scheme", pcsId = model.OrganisationId, complianceYear = model.ComplianceYear });
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
        public async Task<ActionResult> TransferFrom(Guid pcsId, int complianceYear)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId);
                
                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId, complianceYear);
                }

                var model = await TransferFromViewModel(pcsId, complianceYear, client, transferRequest, 1);

                return this.View("TransferFrom", model);
            }
        }

        private async Task<TransferEvidenceNotesViewModel> TransferFromViewModel(Guid pcsId, int complianceYear, IWeeeClient client,
            TransferEvidenceNoteRequest transferRequest, int pageNumber)
        {
            var result = await client.SendAsync(User.GetAccessToken(),
               new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, complianceYear, null, pageNumber));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(complianceYear, result, transferRequest, pcsId, pageNumber);

            var evidenceNoteIds = transferRequest.EvidenceNoteIds;
            if (evidenceNoteIds != null)
            {
                mapperObject.SessionEvidenceNotesId = evidenceNoteIds;
            }

            var model =
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

            model.NoteCount = result.NoteCount;
                
            sessionService.SetTransferSessionObject(Session, model.EvidenceNotesDataListPaged, SessionKeyConstant.PagingTransferViewModelKey);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferFrom(TransferEvidenceNotesViewModel model)
        {
            if (model.Action == ActionEnum.Back)
            {
                UpdateAndSetSelectedNotesInSession(model);

                return RedirectToAction("TransferEvidenceNote", "TransferEvidence", new { pcsId = model.PcsId, complianceYear = model.ComplianceYear });
            }

            if (model.PageNumber.HasValue)
            {
                UpdateAndSetSelectedNotesInSession(model);

                if (ModelState.ContainsKey("Action"))
                {
                    ModelState["Action"].Errors.Clear();
                    ModelState.Clear();
                }

                using (var client = this.apiClient())
                {
                    var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                        SessionKeyConstant.TransferNoteKey);

                    model = await TransferFromViewModel(model.PcsId, model.ComplianceYear, client, transferRequest, model.PageNumber.Value);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    UpdateAndSetSelectedNotesInSession(model);

                    return RedirectToAction("TransferTonnage", "TransferEvidence",
                        new { area = "Scheme", pcsId = model.PcsId, complianceYear = model.ComplianceYear, transferAllTonnage = false });
                }

                if (!model.PageNumber.HasValue)
                {
                    var pagedModel = sessionService.GetTransferSessionObject<PagedList<ViewEvidenceNoteViewModel>>(Session,
                        SessionKeyConstant.PagingTransferViewModelKey);

                    if (pagedModel != null)
                    {
                        model.EvidenceNotesDataListPaged = pagedModel;
                    }
                }
            }

            await SetBreadcrumb(model.PcsId);

            return View("TransferFrom", model);
        }

        [HttpGet]
        [CheckCanCreateTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> TransferTonnage(Guid pcsId, int complianceYear, bool transferAllTonnage = false)
        {
            using (var client = this.apiClient())
            {
                var model = await TransferEvidenceTonnageViewModel(pcsId, transferAllTonnage, complianceYear, client);

                if (model == null)
                {
                    return RedirectToManageEvidence(pcsId, complianceYear);
                }

                return this.View("TransferTonnage", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferTonnage(TransferEvidenceTonnageViewModel model)
        {
            if (model.Action == ActionEnum.Back)
            {
                sessionService.SetTransferSessionObject(Session, model, SessionKeyConstant.EditTransferTonnageViewModelKey);

                return RedirectToAction("TransferFrom", "TransferEvidence", new { pcsId = model.PcsId, complianceYear = model.ComplianceYear });
            }

            using (var client = this.apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session, SessionKeyConstant.TransferNoteKey);

                    var updatedRequest = transferNoteRequestCreator.SelectTonnageToRequest(transferRequest, model);

                    var id = await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    var updateStatus = (NoteUpdatedStatusEnum)updatedRequest.Status;

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = updateStatus;

                    return RedirectToAction("TransferredEvidence", "TransferEvidence",
                        new { pcsId = model.PcsId, evidenceNoteId = id, redirectTab = Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers) });
                }

                var updatedModel = await TransferEvidenceTonnageViewModel(model.PcsId, false, model.ComplianceYear, client);

                return this.View("TransferTonnage", updatedModel);
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

                return this.View("TransferredEvidence", model);
            }
        }

        private async Task<TransferEvidenceTonnageViewModel> TransferEvidenceTonnageViewModel(Guid pcsId, bool transferAllTonnage, int complianceYear, IWeeeClient client)
        {
            await SetBreadcrumb(pcsId);

            var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                SessionKeyConstant.TransferNoteKey);

            if (transferRequest == null)
            {
                return null;
            }

            var existingModel = sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(Session, SessionKeyConstant.EditTransferTonnageViewModelKey);
            sessionService.ClearTransferSessionObject(Session, SessionKeyConstant.EditTransferTonnageViewModelKey);

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, complianceYear, transferRequest.EvidenceNoteIds));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(complianceYear, result, transferRequest, pcsId)
            {
                TransferAllTonnage = transferAllTonnage,
                ExistingTransferTonnageViewModel = existingModel
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

        private void UpdateAndSetSelectedNotesInSession(TransferEvidenceNotesViewModel model)
        {
            var transferRequest =
               sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                   SessionKeyConstant.TransferNoteKey);

            if (transferRequest != null)
            {
                var result = new List<Guid>();

                var alreadySelectedEvidenceNotes = transferRequest.EvidenceNoteIds;

                result.AddRange(alreadySelectedEvidenceNotes);

                var selectedEvidenceNotes =
                model.SelectedEvidenceNotePairs.Where(a => a.Value.Equals(true)).Select(b => b.Key).ToList();

                foreach (var note in selectedEvidenceNotes)
                {
                    if (!result.Contains(note))
                    {
                        result.Add(note);
                    }
                }
              
                var updatedTransferRequest =
                    new TransferEvidenceNoteRequest(model.PcsId, transferRequest.RecipientId, transferRequest.CategoryIds, result);

                sessionService.SetTransferSessionObject(Session, updatedTransferRequest, SessionKeyConstant.TransferNoteKey);
            }
        }
    }
}