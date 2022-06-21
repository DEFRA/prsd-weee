namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Aatf.ViewModels;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Prsd.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.Shared;

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
        public async Task<ActionResult> Index(Guid pcsId, 
            string tab = null,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);
                var scheme = await Cache.FetchSchemePublicInfo(pcsId);

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                if (tab == null)
                {
                    tab = Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence);
                }
                var value = tab.GetValueFromDisplayName<ManageEvidenceNotesDisplayOptions>();

                switch (value)
                {
                    case ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme.Name, currentDate, manageEvidenceNoteViewModel);
                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(pcsId, scheme.Name, currentDate, manageEvidenceNoteViewModel);
                    case ManageEvidenceNotesDisplayOptions.OutgoingTransfers:
                        return await CreateAndPopulateOutgoingTransfersEvidenceViewModel(pcsId, scheme.Name, currentDate, manageEvidenceNoteViewModel);
                    default:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme.Name, currentDate, manageEvidenceNoteViewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(Guid organisationId)
        {
            return RedirectToAction("TransferEvidenceNote", "TransferEvidence", new { pcsId = organisationId });
        }

        private async Task<ActionResult> CreateAndPopulateReviewSubmittedEvidenceViewModel(Guid organisationId, 
            string schemeName, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(organisationId, 
                    new List<NoteStatus>() { NoteStatus.Submitted }, 
                    SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), NoteType.Evidence, false));

                var model = mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                    new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, result, schemeName, currentDate, manageEvidenceNoteViewModel));

                return View("ReviewSubmittedEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid pcsId, 
            string schemeName, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
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
                    }, SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), NoteType.Evidence, false));

                var model = mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                 new ViewAndTransferEvidenceViewModelMapTransfer(pcsId, result, schemeName, currentDate, manageEvidenceNoteViewModel));

                return View("ViewAndTransferEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateOutgoingTransfersEvidenceViewModel(Guid pcsId,
            string schemeName,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                    new GetEvidenceNotesByOrganisationRequest(pcsId, new List<NoteStatus>()
                    {
                        NoteStatus.Draft,
                        NoteStatus.Approved,
                        NoteStatus.Rejected,
                        NoteStatus.Submitted,
                        NoteStatus.Void,
                        NoteStatus.Returned
                    }, SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), NoteType.Transfer, true));

                var model = mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                      new TransferredOutEvidenceNotesViewModelMapTransfer(pcsId, result, schemeName, currentDate, manageEvidenceNoteViewModel));

                return View("OutgoingTransfers", model);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> ReviewEvidenceNote(Guid pcsId, Guid evidenceNoteId, int complianceYear)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                // create the new evidence note schemeName request from note's Guid
                ReviewEvidenceNoteViewModel model = await GetNote(pcsId, evidenceNoteId, client, complianceYear);

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

                    TempData[ViewDataConstant.EvidenceNoteStatus] = (NoteUpdatedStatusEnum)request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("DownloadEvidenceNote", 
                        new { organisationId = model.OrganisationId, evidenceNoteId = request.NoteId, complianceYear = model.ViewEvidenceNoteViewModel.ComplianceYear });
                }

                await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model = await GetNote(model.ViewEvidenceNoteViewModel.SchemeId, model.ViewEvidenceNoteViewModel.Id, client, model.ViewEvidenceNoteViewModel.ComplianceYear);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNote(Guid pcsId, Guid evidenceNoteId, int complianceYear)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus])
                {
                    SchemeId = pcsId,
                    ComplianceYear = complianceYear
                });

                return View(model);
            }
        }

        private int SelectedComplianceYear(DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            return manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0 ? manageEvidenceNoteViewModel.SelectedComplianceYear : currentDate.Year;
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid pcsId, Guid evidenceNoteId, IWeeeClient client, int complianceYear)
        {
            var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

            var result = await client.SendAsync(User.GetAccessToken(), request);

            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null)
            {
                SchemeId = pcsId,
                ComplianceYear = complianceYear
            });

            return model;
        }
    }
}