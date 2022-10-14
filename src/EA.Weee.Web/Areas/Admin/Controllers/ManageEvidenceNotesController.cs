﻿namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Extensions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
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

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ManageEvidenceNotesController : AdminBreadcrumbBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;
        private readonly ISessionService sessionService;

        public ManageEvidenceNotesController(IMapper mapper,
         BreadcrumbService breadcrumb,
         IWeeeCache cache,
         Func<IWeeeClient> apiClient, 
         ConfigurationService configurationService,
         ISessionService sessionService) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.configurationService = configurationService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        [NoCacheFilter]
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
        [NoCacheFilter]
        public async Task<ActionResult> ViewEvidenceNote(Guid evidenceNoteId, int page = 1, string queryString = null)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus], false, this.User));

                ViewBag.Page = page;
                ViewBag.QueryString = queryString;

                return View(model);
            }
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> ViewEvidenceNoteTransfer(Guid evidenceNoteId, int page = 1, bool openedInNewTab = false)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(result.TransferredOrganisationData.Id,
                   result, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification], this.User)
                {
                    OpenedInNewTab = openedInNewTab,
                    Page = page
                });

                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [NoCacheFilter]
        public async Task<ActionResult> VoidEvidenceNote(Guid evidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

                var evidenceNoteData = await client.SendAsync(User.GetAccessToken(), request);

                if (evidenceNoteData.Type == NoteType.Evidence && evidenceNoteData.Status == NoteStatus.Approved)
                {
                    var model = new VoidEvidenceNoteViewModel()
                    {
                        ViewEvidenceNoteViewModel = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, null))
                    };

                    return View("VoidEvidenceNote", model);
                }

                return RedirectToAction(nameof(Index),
                    new { tab = ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString() });
            }
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [NoCacheFilter]
        public async Task<ActionResult> VoidTransferNote(Guid transferEvidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(transferEvidenceNoteId);

                var transferNoteData = await client.SendAsync(User.GetAccessToken(), request);

                if (transferNoteData.Type == NoteType.Transfer && transferNoteData.Status == NoteStatus.Approved)
                {
                    var model = VoidNoteViewModel(transferNoteData);

                    return View("VoidTransferNote", model);
                }

                return RedirectToAction(nameof(Index),
                    new { tab = ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString() });
            }
        }

        private VoidTransferNoteViewModel VoidNoteViewModel(TransferEvidenceNoteData transferNoteData)
        {
            var model = new VoidTransferNoteViewModel()
            {
                ViewTransferNoteViewModel = mapper.Map<ViewTransferNoteViewModel>(
                    new ViewTransferNoteViewModelMapTransfer(transferNoteData.TransferredOrganisationData.Id,
                        transferNoteData, null, null))
            };

            return model;
        }

        [HttpPost]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VoidEvidenceNote(VoidEvidenceNoteViewModel model)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    await client.SendAsync(User.GetAccessToken(), new VoidNoteRequest(model.ViewEvidenceNoteViewModel.Id, model.VoidedReason));

                    TempData[ViewDataConstant.EvidenceNoteStatus] = NoteUpdatedStatusEnum.Void;

                    return RedirectToAction("ViewEvidenceNote", "ManageEvidenceNotes", new { evidenceNoteId = model.ViewEvidenceNoteViewModel.Id });
                }

                var request = new GetEvidenceNoteForInternalUserRequest(model.ViewEvidenceNoteViewModel.Id);

                var evidenceNoteData = await client.SendAsync(User.GetAccessToken(), request);

                var updatedModel = new VoidEvidenceNoteViewModel()
                {
                    ViewEvidenceNoteViewModel = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, null))
                };

                return View("VoidEvidenceNote", updatedModel);
            }
        }

        [HttpPost]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VoidTransferNote(VoidTransferNoteViewModel model)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                if (ModelState.IsValid)
                {
                    await client.SendAsync(User.GetAccessToken(), new VoidNoteRequest(model.ViewTransferNoteViewModel.EvidenceNoteId, model.VoidedReason));

                    TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = NoteUpdatedStatusEnum.Void;

                    return RedirectToAction("ViewEvidenceNoteTransfer", "ManageEvidenceNotes", new { evidenceNoteId = model.ViewTransferNoteViewModel.EvidenceNoteId });
                }

                var request = new GetEvidenceNoteTransfersForInternalUserRequest(model.ViewTransferNoteViewModel.EvidenceNoteId);

                var transferNoteData = await client.SendAsync(User.GetAccessToken(), request);

                var updatedModel = VoidNoteViewModel(transferNoteData);

                return View("VoidTransferNote", updatedModel);
            }
        }

        private async Task<ActionResult> ViewAllEvidenceNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate, int pageNumber)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var complianceYearsList = (await ComplianceYearsList(client, allowedStatuses)).ToList();

            var selectedComplianceYear = SelectedComplianceYear(complianceYearsList, manageEvidenceNoteViewModel);

            var notes = await client.SendAsync(User.GetAccessToken(), new GetAllNotesInternal(new List<NoteType> { NoteType.Evidence }, 
                allowedStatuses, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultInternalPagingPageSize,
                manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.StartDate,
                manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.EndDate,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy, 
                manageEvidenceNoteViewModel?.FilterViewModel.SearchRef));

            var submittedDatesFilterViewModel = mapper.Map<SubmittedDatesFilterViewModel>(
                    new SubmittedDateFilterBase(manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.StartDate, manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.EndDate));

            var schemeData = await client.SendAsync(User.GetAccessToken(),
               new GetOrganisationSchemeDataForFilterRequest(null, selectedComplianceYear));

            var aatfData = await client.SendAsync(User.GetAccessToken(),
                   new GetAllAatfsForComplianceYearRequest(selectedComplianceYear));

            var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                        new RecipientWasteStatusFilterBase(schemeData, manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                        manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue, 
                        manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                        manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy, aatfData, true));

            var model = mapper.Map<ViewAllEvidenceNotesViewModel>(
                new ViewEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, pageNumber, configurationService.CurrentConfiguration.DefaultInternalPagingPageSize,
                complianceYearsList));

            model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(manageEvidenceNoteViewModel?.FilterViewModel, recipientWasteStatusViewModel, submittedDatesFilterViewModel, selectedComplianceYear, currentDate, complianceYearsList));

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
                new GetAllNotesInternal(new List<NoteType> { NoteType.Transfer }, allowedStatuses, selectedComplianceYear, pageNumber, 
                configurationService.CurrentConfiguration.DefaultInternalPagingPageSize,
                manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.StartDate,
                manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.EndDate,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy,
                manageEvidenceNoteViewModel?.FilterViewModel.SearchRef));

            var model = mapper.Map<ViewAllTransferNotesViewModel>(
                new ViewEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, pageNumber, configurationService.CurrentConfiguration.DefaultInternalPagingPageSize, complianceYearsList));

            model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(null, null, null, selectedComplianceYear, currentDate, complianceYearsList));

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