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
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
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

        public ManageEvidenceNotesController(IMapper mapper,
         BreadcrumbService breadcrumb,
         IWeeeCache cache,
         Func<IWeeeClient> apiClient) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string tab = null, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null)
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
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel, currentDate);
                    case ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers:
                        return await ViewAllTransferNotes(client, manageEvidenceNoteViewModel, currentDate);
                    default:
                        return await ViewAllEvidenceNotes(client, manageEvidenceNoteViewModel, currentDate);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewEvidenceNote(Guid evidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewEvidenceNoteTransfer(Guid evidenceNoteId)
        {
            SetBreadcrumb(BreadCrumbConstant.ManageEvidenceNotesAdmin);

            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteTransfersForInternalUserRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewTransferNoteViewModel>(new ViewTransferNoteViewModelMapTransfer(result.TransferredOrganisationData.Id,
                   result, TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification]));

                return View(model);
            }
        }

        private async Task<ActionResult> ViewAllEvidenceNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var complianceYearsList = (await ComplianceYearsList(client, allowedStatuses)).ToList();

            var selectedComplianceYear = SelectedComplianceYear(complianceYearsList, manageEvidenceNoteViewModel);

            var notes = await client.SendAsync(User.GetAccessToken(), new GetAllNotesInternal(new List<NoteType> { NoteType.Evidence }, allowedStatuses, selectedComplianceYear));

            var model = mapper.Map<ViewAllEvidenceNotesViewModel>(new ViewAllEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, complianceYearsList));

            return View("ViewAllEvidenceNotes", model);
        }

        private async Task<IEnumerable<int>> ComplianceYearsList(IWeeeClient client, List<NoteStatus> allowedStatuses)
        {
            return await client.SendAsync(User.GetAccessToken(), new GetComplianceYearsFilter(allowedStatuses));
        }

        private async Task<ActionResult> ViewAllTransferNotes(IWeeeClient client, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDate)
        {
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

            var complianceYearsList = (await ComplianceYearsList(client, allowedStatuses)).ToList();

            var selectedComplianceYear = SelectedComplianceYear(complianceYearsList, manageEvidenceNoteViewModel);

            var notes = await client.SendAsync(User.GetAccessToken(), new GetAllNotesInternal(new List<NoteType> { NoteType.Transfer }, allowedStatuses, selectedComplianceYear));

            var model = mapper.Map<ViewAllTransferNotesViewModel>(new ViewAllEvidenceNotesMapTransfer(notes, manageEvidenceNoteViewModel, currentDate, complianceYearsList));

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