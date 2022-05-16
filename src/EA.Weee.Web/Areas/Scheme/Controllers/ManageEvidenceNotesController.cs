namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Prsd.Core.Helpers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class ManageEvidenceNotesController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid pcsId, string tab = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);
                var scheme = await client.SendAsync(User.GetAccessToken(), new GetSchemeByOrganisationId(pcsId));

                if (tab == null)
                {
                    tab = Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.Summary);
                }
                var value = tab.GetValueFromName<ManageEvidenceNotesDisplayOptions>();
                switch (value)
                {
                    case ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme);
                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(pcsId, scheme);
                    default:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(Guid organisationId)
        {
            return RedirectToAction("TransferEvidenceNote", "TransferEvidence", new { pcsId = organisationId });
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

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid pcsId, SchemeData scheme)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),

                new GetEvidenceNotesByOrganisationRequest(pcsId, new List<NoteStatus>() 
                { 
                    NoteStatus.Approved, 
                    NoteStatus.Rejected,
                    NoteStatus.Void,
                    NoteStatus.Returned
                }));

                var schemeName = scheme != null ? scheme.SchemeName : string.Empty;

                var model = mapper.Map<ViewAndTransferEvidenceViewModel>(new ViewAndTransferEvidenceViewModelMapTransfer(pcsId, result, schemeName));

                return View("ViewAndTransferEvidence", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ReviewEvidenceNote(Guid pcsId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                // create the new evidence note scheme request from note's Guid
                ReviewEvidenceNoteViewModel model = await GetNote(pcsId, evidenceNoteId, client);

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

                    var request = new SetNoteStatus(model.ViewEvidenceNoteViewModel.Id, status, model.Reason);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("DownloadEvidenceNote", new { organisationId = model.ViewEvidenceNoteViewModel.OrganisationId, evidenceNoteId = request.NoteId });
                }

                await SetBreadcrumb(model.ViewEvidenceNoteViewModel.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model = await GetNote(model.ViewEvidenceNoteViewModel.SchemeId, model.ViewEvidenceNoteViewModel.Id, client);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNote(Guid pcsId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                // call the api with the new evidence note scheme request
                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus])
                {
                    SchemeId = pcsId
                });

                //return viewmodel to view
                return View(model);
            }
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid pcsId, Guid evidenceNoteId, IWeeeClient client)
        {
            var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

            // call the api with the new evidence note scheme request
            var result = await client.SendAsync(User.GetAccessToken(), request);

            // create new viewmodel mapper to map request to viewmodel
            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null)
            {
                SchemeId = pcsId
            });

            return model;
        }
    }
}