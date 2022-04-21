namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
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
    using Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Note;

    public class ManageEvidenceNotesController : SchemeEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
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

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
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

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid organisationId, SchemeData scheme)
        {
            using (var client = this.apiClient())
            {
                // TODO: Add NoteStatus Returned to this list
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(organisationId, new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Void }));

                var schemeName = scheme != null ? scheme.SchemeName : string.Empty;

                var model = mapper.Map<ViewAndTransferEvidenceViewModel>(new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, result, schemeName));

                return View("ViewAndTransferEvidence", model);
            }
        [HttpGet]
        public async Task<ActionResult> ReviewEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.SchemeManageEvidence);

                // create the new evidence note scheme request from note's Guid
                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                // call the api with the new evidence note scheme request
                var result = await client.SendAsync(User.GetAccessToken(), request);

                // create new viewmodel mapper to map request to viewmodel
                var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null));
                model.SelectedId = evidenceNoteId;

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
                await SetBreadcrumb(model.ViewEvidenceNoteViewModel.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                var result = ModelState.IsValid;  //TODO: my model [Required] attributes do not trigger ModelState validity

                //TODO: to be replaced by ModelState.IsValid when it works
                if (model.EvidenceNoteApprovalOptionsViewModel != null)
                {
                    //TODO: data do not get commited to db 
                    NoteStatus status = model.EvidenceNoteApprovalOptionsViewModel.SelectedEnumValue;
                    var request = new SetNoteStatus(model.ViewEvidenceNoteViewModel.Id, status);
                    await client.SendAsync(User.GetAccessToken(), request);  

                    //TODO: reading back the data shows that db has not been updated
                    var requestReadback = new GetEvidenceNoteForSchemeRequest(model.SelectedId.Value);
                    var resultReadback = await client.SendAsync(User.GetAccessToken(), requestReadback);
                    model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(resultReadback, null));
                    model.ViewEvidenceNoteViewModel.Status = NoteStatus.Approved;        //TODO: this info should written to db
                    model.ApprovedDate = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT)"; //TODO: this info should written to db - data type change for DisplayFor
                    model.ShowRadioButtonsDisplay = false;
                }

                return View("ReviewEvidenceNote", model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }
    }
}