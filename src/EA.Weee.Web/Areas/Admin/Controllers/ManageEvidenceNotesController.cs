namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using Prsd.Core;

    public class ManageEvidenceNotesController : AdminBreadcrumbBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;

        public ManageEvidenceNotesController(IMapper mapper,
         BreadcrumbService breadcrumb,
         IWeeeCache cache,
         Func<IWeeeClient> apiClient, ConfigurationService configurationService) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.configurationService = configurationService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string tab = null, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null, int page = 1)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            if (tab == null)
            {
                tab = Extensions.ToDisplayString(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes);
            }

            var value = tab.GetValueFromDisplayName<ManageEvidenceNotesTabDisplayOptions>();

            using (var client = this.apiClient())
            {
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
               
                switch (value)
                {
                    case ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes:
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel, currentDate, page);
                    case ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers:
                        return await ViewAllTransferNotes(client, manageEvidenceNoteViewModel, currentDate, page);
                    default:
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel, currentDate, page);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewEvidenceNote(Guid evidenceNoteId, int page = 1)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                ViewBag.Page = page;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewEvidenceNoteTransfer(Guid evidenceNoteId, int page = 1)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(result.TransferredOrganisationData.Id,
                   result, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification], this.User));

                ViewBag.Page = page;

                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> VoidTransferNote(Guid transferEvidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(transferEvidenceNoteId);

                var transferNoteData = await client.SendAsync(User.GetAccessToken(), request);

                if (transferNoteData.Type == NoteType.Transfer && transferNoteData.Status == NoteStatus.Approved)
                {
                    var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(transferNoteData.TransferredOrganisationData.Id,
                       transferNoteData, null, this.User));

                    return View("VoidTransferNote", model);
                }

                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "This note is not a transfer note or has not been approved.");
            }
        }

        [HttpPost]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VoidTransferNote(ViewTransferNoteViewModel model)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(model.EvidenceNoteId);

                await client.SendAsync(User.GetAccessToken(), new VoidTransferNoteRequest(model.EvidenceNoteId, model.VoidedReason));

                var transferNoteData = await client.SendAsync(User.GetAccessToken(), request);

                if (transferNoteData.Type == NoteType.Transfer && transferNoteData.Status == NoteStatus.Void)
                {
                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = NoteUpdatedStatusEnum.Void;

                    model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(transferNoteData.TransferredOrganisationData.Id,
                        transferNoteData, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification], this.User));

                    return RedirectToAction("ViewEvidenceNoteTransfer", "ManageEvidenceNotes", new { evidenceNoteId = model.EvidenceNoteId });
                }
            }

            return View("VoidTransferNote", model);
        }

        private async Task<ActionResult> ViewAllEvidenceNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate, int pageNumber)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var complianceYearsList = (await ComplianceYearsList(client, allowedStatuses)).ToList();

            var selectedComplianceYear = SelectedComplianceYear(complianceYearsList, manageEvidenceNoteViewModel);

            var notes = await client.SendAsync(User.GetAccessToken(), new GetAllNotesInternal(new List<NoteType> { NoteType.Evidence }, allowedStatuses, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultPagingPageSize));

            var model = mapper.Map<ViewAllEvidenceNotesViewModel>(new ViewEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, pageNumber, complianceYearsList));

            return View("ViewAllEvidenceNotes", model);
        }

        private async Task<IEnumerable<int>> ComplianceYearsList(IWeeeClient client, List<NoteStatus> allowedStatuses)
        {
            return await client.SendAsync(User.GetAccessToken(), new GetComplianceYearsFilter(allowedStatuses));
        }

        private async Task<ActionResult> ViewAllTransferNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate, int pageNumber)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var complianceYearsList = (await ComplianceYearsList(client, allowedStatuses)).ToList();

            var selectedComplianceYear = SelectedComplianceYear(complianceYearsList, manageEvidenceNoteViewModel);

            var notes = await client.SendAsync(User.GetAccessToken(), 
                new GetAllNotesInternal(new List<NoteType> { NoteType.Transfer }, allowedStatuses, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultPagingPageSize));

            var model = mapper.Map<ViewAllTransferNotesViewModel>(new ViewEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, pageNumber, complianceYearsList));

            return View("ViewAllTransferNotes", model);
        }

        private static int SelectedComplianceYear(IReadOnlyCollection<int> complianceYearsList, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            return manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0
                    ? manageEvidenceNoteViewModel.SelectedComplianceYear
                    : (complianceYearsList.Any() ? complianceYearsList.ElementAt(0) : SystemTime.Now.Year);
        }
    }
}