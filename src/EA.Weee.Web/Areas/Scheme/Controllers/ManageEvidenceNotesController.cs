namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class ManageEvidenceNotesController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient,
            IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
            this.transferNoteRequestCreator = transferNoteRequestCreator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, ManageEvidenceNotesDisplayOptions? activeDisplayOption = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeByOrganisationId(organisationId));

                switch (activeDisplayOption)
                {
                    case ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(organisationId, scheme);
                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(organisationId, scheme);
                    default:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(organisationId, scheme);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(Guid organisationId)
        {
            return RedirectToAction("TransferEvidenceNote", "ManageEvidenceNotes", new { area = "Scheme", organisationId });
        }

        [HttpGet]
        public async Task<ActionResult> TransferEvidenceNote(Guid organisationId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                var model = new TransferEvidenceNoteDataViewModel();
                model.OrganisationId = organisationId;
                model.SchemasToDisplay = await GetApprovedSchemes();
                return this.View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TransferEvidenceNote(TransferEvidenceNoteDataViewModel model)
        {
            var ids = model.CategoryValues.Where(c => c.Selected).Select(c => c.CategoryId).ToList();

            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    var transferRequest = transferNoteRequestCreator.ViewModelToRequest(model);

                    var sessionId = $"TransferEvidenceNoteData_{User.GetUserId()}_{model.SelectedSchema.Value}";
                    Session[sessionId] = transferRequest;

                    return RedirectToAction("Index", "Holding", new { Area = "Aatf", OrganisationId = model.OrganisationId });
                }

                await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model.AddCategoryValues();
                CheckedCategoryIds(model, ids);
                model.SchemasToDisplay = await GetApprovedSchemes();

                return View(model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateReviewSubmittedEvidenceViewModel(Guid organisationId, SchemeData scheme)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(organisationId, new List<NoteStatus>() { NoteStatus.Submitted }));

                var schemeName = scheme != null ? scheme.SchemeName : string.Empty;

                var model = mapper.Map<ReviewSubmittedEvidenceNotesViewModel>(new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, result, schemeName));

                return View("ReviewSubmittedEvidence", model);
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

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid organisationId, SchemeData scheme)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),

                new GetEvidenceNotesByOrganisationRequest(organisationId, new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Void }));

                var schemeName = scheme != null ? scheme.SchemeName : string.Empty;

                var model = mapper.Map<ViewAndTransferEvidenceViewModel>(new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, result, schemeName));

                return View("ViewAndTransferEvidence", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ReviewEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                // create the new evidence note scheme request from note's Guid
                var model = await GetNote(evidenceNoteId, client);

                //return viewmodel to view
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

                    if (status == NoteStatus.Approved)
                    {
                        model.ViewEvidenceNoteViewModel.SuccessMessage = "Your evidence note has successfully been set to 'Approved'.";
                    }

                    var request = new SetNoteStatus(model.ViewEvidenceNoteViewModel.Id, status);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("DownloadEvidenceNote", new { organisationId = model.ViewEvidenceNoteViewModel.OrganisationId, evidenceNoteId = request.NoteId });
                }

                await SetBreadcrumb(model.ViewEvidenceNoteViewModel.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model = await GetNote(model.ViewEvidenceNoteViewModel.Id, client);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                // call the api with the new evidence note scheme request
                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                //return viewmodel to view
                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid evidenceNoteId, IWeeeClient client)
        {
            var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

            // call the api with the new evidence note scheme request
            var result = await client.SendAsync(User.GetAccessToken(), request);

            // create new viewmodel mapper to map request to viewmodel
            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null));

            return model;
        }

        private void CheckedCategoryIds(TransferEvidenceNoteDataViewModel model, List<int> ids)
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
    }
}