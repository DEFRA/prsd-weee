namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Constant;
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Scheme;
    using Infrastructure;
    using Mappings.ToViewModels;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfEvidence;

    public class TransferEvidenceController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator;
        private readonly ISessionService sessionService;
        private readonly IWeeeCache cache;

        public TransferEvidenceController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper, IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator, IWeeeCache cache, ISessionService sessionService) : base(breadcrumb, cache)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
            this.transferNoteRequestCreator = transferNoteRequestCreator;
            this.cache = cache;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult> TransferEvidenceNote(Guid pcsId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var model = new TransferEvidenceNoteCategoriesViewModel();
                model.OrganisationId = pcsId;
                model.SchemasToDisplay = await GetApprovedSchemes();
                return this.View("TransferEvidenceNote", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteCategoriesViewModel model)
        {
            var selectedCategoryIds = model.SelectedCategoryValues;

            if (ModelState.IsValid)
            {
                var transferRequest = transferNoteRequestCreator.ViewModelToRequest(model);

                sessionService.SetTransferSessionObject(Session, transferRequest, SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("TransferFrom", "TransferEvidence", new { area = "Scheme", pcsId = model.OrganisationId });
            }

            await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

            model.AddCategoryValues();
            CheckedCategoryIds(model, selectedCategoryIds);
            model.SchemasToDisplay = await GetApprovedSchemes();

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

                Condition.Requires(transferRequest).IsNotNull("Transfer categories not found");

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
                // TODO update the request values with notes captured on the form
                var transferRequest =
                    sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                        SessionKeyConstant.TransferNoteKey);

                sessionService.SetTransferSessionObject(Session, transferRequest, SessionKeyConstant.TransferNoteKey);

                return RedirectToAction("Index", "Holding", new { area = "Scheme", organisationId = model.PcsId });
            }

            await SetBreadcrumb(model.PcsId, BreadCrumbConstant.SchemeManageEvidence);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> TransferTonnage(Guid pcsId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var transferRequest = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(Session,
                    SessionKeyConstant.TransferNoteKey);

                Condition.Requires(transferRequest).IsNotNull("Transfer categories not found");

                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesForTransferRequest(pcsId, transferRequest.CategoryIds));

                var mapperObject = new TransferEvidenceNotesViewModelMapTransfer(result, transferRequest, pcsId);

                var model =
                    mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(mapperObject);

                return this.View("TransferTonnage", model);
            }
        }

        private void CheckedCategoryIds(TransferEvidenceNoteCategoriesViewModel model, List<int> ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    for (int i = 0; i < model.CategoryValues.Count; i++)
                    {
                        if (model.CategoryValues[i].CategoryId == id)
                        {
                            model.CategoryValues[i].Selected = true;
                        }
                    }
                }
            }
        }

        private async Task<List<SchemeData>> GetApprovedSchemes()
        {
            using (var client = apiClient())
            {
                var schemes = await client.SendAsync(User.GetAccessToken(), new GetSchemesExternal(false));
                return schemes;
            }
        }
    }
}