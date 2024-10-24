﻿namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Attributes;
    using Core.Helpers;
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
    using Filters;
    using Requests;
    using ViewModels;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class OutgoingTransfersController : TransferEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ITransferEvidenceRequestCreator transferEvidenceRequestCreator;
        private readonly ConfigurationService configurationService;

        public OutgoingTransfersController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient, 
            ISessionService sessionService,
            ITransferEvidenceRequestCreator transferEvidenceRequestCreator,
            ConfigurationService configurationService) : base(breadcrumb, cache, sessionService)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.transferEvidenceRequestCreator = transferEvidenceRequestCreator;
            this.configurationService = configurationService;
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> EditTonnages(Guid pcsId, Guid evidenceNoteId, bool? returnToEditDraftTransfer)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                if (noteData.Status != NoteStatus.Returned && noteData.Status != NoteStatus.Draft)
                {
                    // we redirect to manage evidence notes tab if this note already has been processed (clicked on Browser Back Button)
                    return RedirectToAction("Index", "ManageEvidenceNotes",
                        new { pcsId, area = "Scheme", tab = ManageEvidenceNotesDisplayOptions.OutgoingTransfers.ToDisplayString() });
                }

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
                SessionService.SetTransferSessionObject(model, SessionKeyConstant.EditTransferEvidenceTonnageViewModel);

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
                    var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

                    var updatedRequest = transferEvidenceRequestCreator.EditSelectTonnageToRequest(transferRequest, model);

                    var route = model.Action == ActionEnum.Submit
                        ? SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName :
                            SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName;

                    await client.SendAsync(User.GetAccessToken(), updatedRequest);

                    var updateStatus = GetUpdateStatus(model, updatedRequest);

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

        private NoteUpdatedStatusEnum GetUpdateStatus(TransferEvidenceTonnageViewModel model,
            EditTransferEvidenceNoteRequest updatedRequest)
        {
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

            return updateStatus;
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> EditDraftTransfer(Guid pcsId, Guid evidenceNoteId, bool? returnToView, string redirectTab = null, int page = 1, string queryString = null)
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
                    SystemDateTime = currentDate,
                    QueryString = queryString
                });

                ViewBag.Page = page;

                return this.View("EditDraftTransfer", model);
            }
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
        public async Task<ActionResult> SubmittedTransfer(Guid pcsId, Guid evidenceNoteId, bool? returnToView, string redirectTab, string queryString = null)
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
                    QueryString = queryString
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
        [NoCacheFilter]
        public async Task<ActionResult> EditTransferFrom(Guid pcsId, Guid evidenceNoteId, int page = 1, string searchRef = null, Guid? submittedBy = null)
        {
            await SetBreadcrumb(pcsId);

            using (var client = apiClient())
            {
                var transferRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

                if (transferRequest == null)
                {
                    return RedirectToManageEvidence(pcsId);
                }

                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var model = await TransferEvidenceNotesViewModel(pcsId, page, client, transferRequest, searchRef, submittedBy, noteData);

                return this.View("EditTransferFrom", model);
            }
        }

        private async Task<TransferEvidenceNotesViewModel> TransferEvidenceNotesViewModel(Guid pcsId, 
            int page, 
            IWeeeClient client, 
            TransferEvidenceNoteRequest transferRequest, 
            string searchRef,
            Guid? submittedBy,
            TransferEvidenceNoteData noteData)
        {
            transferRequest.UpdateSelectedNotes(noteData.CurrentEvidenceNoteIds);

            var currentSelectedNotes = new EvidenceNoteSearchDataResult();
            if (transferRequest.EvidenceNoteIds.Any())
            {
                currentSelectedNotes = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesSelectedForTransferRequest(pcsId, transferRequest.EvidenceNoteIds, transferRequest.CategoryIds, noteData.Id));
            }

            var availableNotes = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds, noteData.ComplianceYear, transferRequest.EvidenceNoteIds, 
                searchRef, submittedBy, page, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize, noteData.Id));

            var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(currentSelectedNotes, availableNotes,
                transferRequest, noteData, pcsId, searchRef, page, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize);

            var model =
                mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(mapperObject);

            model.SubmittedByList = await GetSubmittedByList(pcsId, transferRequest.CategoryIds, noteData.ComplianceYear, 
                transferRequest.EvidenceNoteIds, noteData.Id, client);

            return model;
        }

        private async Task<List<SelectListItem>> GetSubmittedByList(Guid pcsId, List<int> categoryIds, int complianceYear, 
            List<Guid> evidenceNoteIds, Guid id, IWeeeClient client)
        {
            var res = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesForTransferRequest(pcsId, categoryIds, complianceYear, evidenceNoteIds, null, null, 1, int.MaxValue, id));

            var submittedByFilterList = new List<SelectListItem>();
            foreach (var evidenceNoteResult in res.Results)
            {
                (Guid Id, string Name) submittedBy = GetSubmittedBy(evidenceNoteResult);
                if (!submittedByFilterList.Any(x => x.Text == submittedBy.Name) && !string.IsNullOrWhiteSpace(submittedBy.Name))
                {
                    submittedByFilterList.Add(new SelectListItem()
                    {
                        Value = submittedBy.Id.ToString(),
                        Text = submittedBy.Name
                    });
                }
            }

            return submittedByFilterList.OrderBy(x => x.Text).ToList();
        }

        private static (Guid, string) GetSubmittedBy(EvidenceNoteData evidenceNoteResult)
        {
            if (evidenceNoteResult.Type == NoteType.Transfer)
            {
                if (evidenceNoteResult.OrganisationSchemaData != null)
                {
                    return (evidenceNoteResult.OrganisationSchemaData.Id, evidenceNoteResult.OrganisationSchemaData.SchemeName);
                }
                else
                {
                    return (evidenceNoteResult.OrganisationData.Id, evidenceNoteResult.OrganisationData.OrganisationName);
                }
            }
            else if (evidenceNoteResult.SubmittedDate.HasValue)
            {
                return (evidenceNoteResult.AatfData.Id, evidenceNoteResult.AatfData.Name);
            }
            else
            {
                return (Guid.Empty, null);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransferFrom(TransferEvidenceNotesViewModel model)
        {
            await SetBreadcrumb(model.PcsId);

            var outgoingTransfer = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

            if (outgoingTransfer == null)
            {
                return RedirectToManageEvidence(model.PcsId);
            }

            outgoingTransfer.UpdateSelectedNotes(model.EvidenceNotesDataList.Select(e => e.Id).ToList());

            if (!ModelState.IsValid)
            {
                using (var client = this.apiClient())
                {
                    var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(model.ViewTransferNoteViewModel.EvidenceNoteId));

                    model = await TransferEvidenceNotesViewModel(model.PcsId, model.PageNumber, client, outgoingTransfer, null, null, noteData);
                }

                return View("EditTransferFrom", model);
            }

            return RedirectToRoute("Scheme_edit_transfer_tonnages",
                    new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId, returnToEditDraftTransfer = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<ActionResult> SelectEvidenceNote(TransferSelectEvidenceNoteModel model, string searchRef = null, Guid? submittedBy = null)
        {
            await SetBreadcrumb(model.PcsId);

            var transferRequest = SelectEvidenceNote(model.SelectedEvidenceNoteId, SessionKeyConstant.OutgoingTransferKey);

            if (ModelState.IsValid)
            {
                return RedirectToAction("EditTransferFrom", new { pcsId = model.PcsId, EvidenceNoteId = model.EditEvidenceNoteId, page = model.NewPage });
            }

            using (var client = apiClient())
            {
                var noteData = await client.SendAsync(User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(model.EditEvidenceNoteId));

                var newModel = await TransferEvidenceNotesViewModel(model.PcsId, model.Page, client, transferRequest, searchRef, submittedBy, noteData);

                return View("EditTransferFrom", newModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult DeselectEvidenceNote(TransferDeselectEvidenceNoteModel model)
        {
            DeselectEvidenceNote(model.DeselectedEvidenceNoteId, SessionKeyConstant.OutgoingTransferKey);

            return RedirectToAction("EditTransferFrom", new { pcsId = model.PcsId, evidenceNoteId = model.EditEvidenceNoteId, model.Page });
        }

        [HttpGet]
        [CheckCanEditTransferNote]
        [NoCacheFilter]
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
                var existingRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

                var transferRequest = transferEvidenceRequestCreator.SelectCategoriesToRequest(model, existingRequest);

                SessionService.SetTransferSessionObject(transferRequest, SessionKeyConstant.OutgoingTransferKey);

                return RedirectToRoute("Scheme_edit_transfer_notes",
                    new { pcsId = model.PcsId, evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
            }

            using (var client = apiClient())
            {
                var refreshedModel = await TransferEvidenceNoteCategoriesViewModel(model.PcsId, model.ViewTransferNoteViewModel.EvidenceNoteId, client, model);

                return this.View("EditCategories", refreshedModel);
            }
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
                SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

            var existingModel = SessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(SessionKeyConstant.EditTransferEvidenceTonnageViewModel);
            SessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferEvidenceTonnageViewModel);

            var existingEvidenceNoteIds = noteData.TransferEvidenceNoteTonnageData.Select(t => t.OriginalNoteId).ToList();

            if (request?.EvidenceNoteIds != null)
            {
                existingEvidenceNoteIds = existingEvidenceNoteIds.Union(request.EvidenceNoteIds).Distinct().ToList();
                existingEvidenceNoteIds.RemoveAll(a => !request.EvidenceNoteIds.Contains(a));
            }

            var categoryIds = request != null ? request.CategoryIds : noteData.CategoryIds;

            var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesSelectedForTransferRequest(pcsId, existingEvidenceNoteIds, categoryIds));

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
            var existingRequest = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(SessionKeyConstant.OutgoingTransferKey);

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
            SessionService.ClearTransferSessionObject(SessionKeyConstant.OutgoingTransferKey);
            SessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferEvidenceTonnageViewModel);
        }
    }
}