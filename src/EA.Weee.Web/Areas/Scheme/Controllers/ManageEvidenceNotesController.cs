namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Extensions;
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
        private readonly ISessionService sessionService;

        public ManageEvidenceNotesController(IMapper mapper,
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
        public async Task<ActionResult> Index(Guid pcsId, 
            string tab = null,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null)
        {
            sessionService.ClearTransferSessionObject(Session, SessionKeyConstant.TransferNoteKey);

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
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel);
                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel);
                    case ManageEvidenceNotesDisplayOptions.OutgoingTransfers:
                        return await CreateAndPopulateOutgoingTransfersEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel);
                    default:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel);
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
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(organisationId, 
                    new List<NoteStatus>() { NoteStatus.Submitted }, 
                    SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), new List<NoteType>() { NoteType.Evidence, NoteType.Transfer }, false));

                var model = mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                    new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, result, scheme, currentDate, manageEvidenceNoteViewModel));

                return View("ReviewSubmittedEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid pcsId,
            SchemePublicInfo scheme,
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
                    }, SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), new List<NoteType>() { NoteType.Evidence, NoteType.Transfer }, false));

                var model = mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                 new ViewAndTransferEvidenceViewModelMapTransfer(pcsId, result, scheme, currentDate, manageEvidenceNoteViewModel));

                return View("ViewAndTransferEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateOutgoingTransfersEvidenceViewModel(Guid pcsId,
            SchemePublicInfo scheme,
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
                    }, SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel), new List<NoteType>() { NoteType.Transfer }, true));

                var model = mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                      new TransferredOutEvidenceNotesViewModelMapTransfer(pcsId, result, scheme, currentDate, manageEvidenceNoteViewModel));

                return View("OutgoingTransfers", model);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> ReviewEvidenceNote(Guid pcsId, Guid evidenceNoteId, int selectedComplianceYear)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                // create the new evidence note schemeName request from note's Guid
                ReviewEvidenceNoteViewModel model = await GetNote(pcsId, evidenceNoteId, client, selectedComplianceYear);

                if (model.ViewEvidenceNoteViewModel.Status != NoteStatus.Submitted)
                {
                    return RedirectToAction("Index", "ManageEvidenceNotes", new { pcsId, @tab = ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString(), selectedComplianceYear });
                }

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
                        new { organisationId = model.OrganisationId, evidenceNoteId = request.NoteId, selectedComplianceYear = model.ViewEvidenceNoteViewModel.ComplianceYear });
                }

                await SetBreadcrumb(model.OrganisationId, BreadCrumbConstant.SchemeManageEvidence);

                model = await GetNote(model.ViewEvidenceNoteViewModel.SchemeId, model.ViewEvidenceNoteViewModel.Id, client, model.ViewEvidenceNoteViewModel.SelectedComplianceYear);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNote(Guid pcsId, Guid evidenceNoteId, int selectedComplianceYear, string redirectTab = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId, BreadCrumbConstant.SchemeManageEvidence);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus])
                {
                    SchemeId = pcsId,
                    SelectedComplianceYear = selectedComplianceYear,
                    RedirectTab = redirectTab
                });

                return View(model);
            }
        }

        private int SelectedComplianceYear(DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            var selectedComplianceYear = sessionService.GetTransferSessionObject<object>(Session, SessionKeyConstant.SchemeSelectedComplianceYear);

            var complianceYear = manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0 ? manageEvidenceNoteViewModel.SelectedComplianceYear : (selectedComplianceYear == null ? currentDate.Year : (int)selectedComplianceYear);

            sessionService.SetTransferSessionObject(Session, complianceYear, SessionKeyConstant.SchemeSelectedComplianceYear);

            return complianceYear;
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid pcsId, Guid evidenceNoteId, IWeeeClient client, int complianceYear)
        {
            var result = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNoteForSchemeRequest(evidenceNoteId));

            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null)
            {
                SchemeId = pcsId,
                SelectedComplianceYear = complianceYear
            });

            return model;
        }
    }
}