namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using AatfEvidence.Controllers;
    using Api.Client;
    using Attributes;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Helpers;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Constant;
    using Extensions;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using ViewModels;
    using Web.Requests.Base;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Scheme;

    public class ManageEvidenceNotesController : AatfEvidenceBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IRequestCreator<EditEvidenceNoteViewModel, CreateEvidenceNoteRequest> createRequestCreator;
        private readonly IRequestCreator<EditEvidenceNoteViewModel, EditEvidenceNoteRequest> editRequestCreator;
        private readonly ISessionService sessionService;

        public ManageEvidenceNotesController(IMapper mapper, 
            BreadcrumbService breadcrumb, 
            IWeeeCache cache, 
            Func<IWeeeClient> apiClient, 
            IRequestCreator<EditEvidenceNoteViewModel, CreateEvidenceNoteRequest> createRequestCreator, 
            IRequestCreator<EditEvidenceNoteViewModel, EditEvidenceNoteRequest> editRequestCreator,
            ISessionService sessionService)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
            this.createRequestCreator = createRequestCreator;
            this.editRequestCreator = editRequestCreator;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid organisationId, Guid aatfId, 
            string tab = null,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null)
        {
            await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);
            
            if (tab == null)
            {
                tab = Extensions.ToDisplayString(ManageEvidenceOverviewDisplayOption.EvidenceSummary);
            }

            var value = tab.GetValueFromDisplayName<ManageEvidenceOverviewDisplayOption>();

            using (var client = this.apiClient())
            {
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
                int selectedComplianceYear = SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel);

                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(aatfId));
                var allAatfsAndAes = await client.SendAsync(User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                switch (value)
                {
                    case ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes:

                        return await EditDraftReturnNoteCase(client, 
                            organisationId, 
                            aatfId, 
                            aatf,
                            allAatfsAndAes,
                            currentDate,
                            selectedComplianceYear,
                            manageEvidenceNoteViewModel);

                    case ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes:

                        return await ViewAllOtherEvidenceNotesCase(organisationId, aatfId, client, aatf, allAatfsAndAes, currentDate, selectedComplianceYear, manageEvidenceNoteViewModel);

                    case ManageEvidenceOverviewDisplayOption.EvidenceSummary:
                    default:
                        return await EvidenceSummaryCase(organisationId, aatfId, client, aatf, allAatfsAndAes, selectedComplianceYear, currentDate);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ManageEvidenceNoteViewModel model)
        {
            return RedirectToAction("CreateEvidenceNote", "ManageEvidenceNotes", new { area = "Aatf", model.OrganisationId, model.AatfId, complianceYear = model.SelectedComplianceYear });
        }

        [HttpGet]
        [CheckCanCreateEvidenceNote]

        public async Task<ActionResult> CreateEvidenceNote(Guid organisationId, Guid aatfId, int complianceYear, bool returnFromCopyPaste = false)
        { 
            using (var client = apiClient())
            {
                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));

                var existingModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(Session, SessionKeyConstant.EditEvidenceViewModelKey);

                sessionService.SetTransferSessionObject(Session, null, SessionKeyConstant.EditEvidenceViewModelKey);

                var model = !returnFromCopyPaste ? mapper.Map<EditEvidenceNoteViewModel>(new CreateNoteMapTransfer(organisationSchemes, null, organisationId, aatfId, complianceYear)) 
                    : mapper.Map<EditEvidenceNoteViewModel>(new CreateNoteMapTransfer(organisationSchemes, existingModel, organisationId, aatfId, complianceYear));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);
            
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvidenceNote(EditEvidenceNoteViewModel viewModel, Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                if (viewModel.Action == ActionEnum.CopyAndPaste)
                {
                    sessionService.SetTransferSessionObject(Session, viewModel, SessionKeyConstant.EditEvidenceViewModelKey);
                    return RedirectToAction("Index", EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName, new { organisationId, returnAction = EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction, complianceYear = viewModel.ComplianceYear });
                }

                if (ModelState.IsValid)
                {
                    var request = createRequestCreator.ViewModelToRequest(viewModel);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = (NoteUpdatedStatusEnum)request.Status;

                    try
                    {
                        var result = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectAfterNoteAction(organisationId, aatfId, request.Status, result);
                    }
                    catch (ApiException ex)
                    {
                        if (ex.ErrorData.ExceptionType == typeof(InvalidOperationException).FullName)
                        {
                            ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));
                
                var model = mapper.Map<EditEvidenceNoteViewModel>(new CreateNoteMapTransfer(organisationSchemes, viewModel, organisationId, aatfId, viewModel.ComplianceYear));

                ModelState.ApplyCustomValidationSummaryOrdering(EditEvidenceNoteViewModel.ValidationMessageDisplayOrder);

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewDraftEvidenceNote(Guid organisationId, Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                var request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus]));

                return View(model);
            }
        }

        [HttpGet]
        [CheckCanEditEvidenceNote]
        public async Task<ActionResult> EditEvidenceNote(Guid organisationId, Guid evidenceNoteId, bool returnFromCopyPaste = false)
        {
            using (var client = apiClient())
            {
                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));

                var existingModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(Session, SessionKeyConstant.EditEvidenceViewModelKey);
                sessionService.SetTransferSessionObject(Session, null, SessionKeyConstant.EditEvidenceViewModelKey);

                var request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);
                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = !returnFromCopyPaste ? mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, null, organisationId, result.AatfData.Id, result, result.ComplianceYear))
                    : mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, existingModel, organisationId, result.AatfData.Id, result, result.ComplianceYear));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckCanEditEvidenceNote]
        public async Task<ActionResult> EditEvidenceNote(EditEvidenceNoteViewModel viewModel, Guid organisationId, Guid aatfId)
        {
            using (var client = apiClient())
            {
                if (viewModel.Action == ActionEnum.CopyAndPaste)
                {
                    sessionService.SetTransferSessionObject(Session, viewModel, SessionKeyConstant.EditEvidenceViewModelKey);
                    return RedirectToAction("Index", EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName, new { organisationId, returnAction = EvidenceCopyPasteActionConstants.EditEvidenceNoteAction });
                }
                if (ModelState.IsValid)
                {
                    var request = editRequestCreator.ViewModelToRequest(viewModel);

                    var updateStatus = request.Status == NoteStatus.Returned && viewModel.Action == ActionEnum.Save ? NoteUpdatedStatusEnum.ReturnedSaved : (NoteUpdatedStatusEnum)request.Status;

                    TempData[ViewDataConstant.EvidenceNoteStatus] = updateStatus;

                    try
                    {
                        var result = await client.SendAsync(User.GetAccessToken(), request);

                        return RedirectAfterNoteAction(organisationId, aatfId, request.Status, result);
                    }
                    catch (ApiException ex)
                    {
                        if (ex.ErrorData.ExceptionType == typeof(InvalidOperationException).FullName)
                        {
                            ModelState.AddModelError(string.Empty, ex.ErrorData.ExceptionMessage);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));

                var model = mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, viewModel, organisationId, aatfId, null, viewModel.ComplianceYear));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            breadcrumb.ExternalOrganisation = await cache.FetchOrganisationName(organisationId);
            breadcrumb.ExternalActivity = activity;
            breadcrumb.OrganisationId = organisationId;
        }

        private ActionResult RedirectAfterNoteAction(Guid organisationId, Guid aatfId, NoteStatus status, Guid result)
        {
            var routeName = status == NoteStatus.Draft
                ? AatfEvidenceRedirect.ViewDraftEvidenceRouteName
                : AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;

            return RedirectToRoute(routeName, new
            {
                organisationId,
                aatfId,
                evidenceNoteId = result
            });
        }
        
        private async Task<ActionResult> ViewAllOtherEvidenceNotesCase(Guid organisationId, Guid aatfId, IWeeeClient client, AatfData aatf,
            List<AatfData> allAatfs, DateTime currentDate, int selectedComplianceYear, ManageEvidenceNoteViewModel manageEvidenceViewModel)
        {
            EvidenceNoteSearchDataResult resultAllNotes = new EvidenceNoteSearchDataResult();

            if (ModelState.IsValid)
            {
               resultAllNotes = await client.SendAsync(User.GetAccessToken(), new GetAatfNotesRequest(organisationId, aatfId, new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected },
               manageEvidenceViewModel?.FilterViewModel.SearchRef,
               selectedComplianceYear,
               manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
               manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
               manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
               manageEvidenceViewModel?.SubmittedDatesFilterViewModel.StartDate,
               manageEvidenceViewModel?.SubmittedDatesFilterViewModel.EndDate));
            }

            var modelAllNotes = mapper.Map<AllOtherManageEvidenceNotesViewModel>(new EvidenceNotesViewModelTransfer(organisationId, aatfId, resultAllNotes, currentDate, manageEvidenceViewModel));

            var schemeData = resultAllNotes.Results.ToList().CreateOrganisationSchemeDataList();

            if (schemeData.Any())
            {
                sessionService.SetTransferSessionObject(Session, schemeData, SessionKeyConstant.FilterRecipientNameKey);
            }

            schemeData = sessionService.GetTransferSessionObject<List<OrganisationSchemeData>>(Session, SessionKeyConstant.FilterRecipientNameKey);

            var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                        new RecipientWasteStatusFilterBase(schemeData, manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId, 
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue, manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue));

            var submittedDatesFilterViewModel = mapper.Map<SubmittedDatesFilterViewModel>(
                        new SubmittedDateFilterBase(manageEvidenceViewModel?.SubmittedDatesFilterViewModel.StartDate, manageEvidenceViewModel?.SubmittedDatesFilterViewModel.EndDate));

            modelAllNotes.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, allAatfs, manageEvidenceViewModel?.FilterViewModel, recipientWasteStatusViewModel, submittedDatesFilterViewModel, selectedComplianceYear, currentDate));

            return this.View("Overview/ViewAllOtherEvidenceOverview", modelAllNotes);
        }

        private async Task<ActionResult> EvidenceSummaryCase(Guid organisationId, Guid aatfId, IWeeeClient client, AatfData aatf, List<AatfData> allAatfs, int selectedComplianceYear, DateTime currentDate)
        {
            var request = new GetAatfSummaryRequest(aatfId, selectedComplianceYear);
            var result = await client.SendAsync(User.GetAccessToken(), request);

            var summaryModel = mapper.Map<ManageEvidenceSummaryViewModel>(new EvidenceSummaryMapTransfer(organisationId, aatfId, result));

            summaryModel.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, allAatfs, null, null, null, selectedComplianceYear, currentDate));

            return this.View("Overview/EvidenceSummaryOverview", summaryModel);
        }

        private int SelectedComplianceYear(DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            var selectedComplianceYear = sessionService.GetTransferSessionObject<object>(Session, SessionKeyConstant.AatfSelectedComplianceYear);

            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(manageEvidenceNoteViewModel, selectedComplianceYear, currentDate);

            sessionService.SetTransferSessionObject(Session, complianceYear, SessionKeyConstant.AatfSelectedComplianceYear);

            return complianceYear;
        }

        private async Task<ActionResult> EditDraftReturnNoteCase(IWeeeClient client, 
            Guid organisationId, 
            Guid aatfId, 
            AatfData aatf, 
            List<AatfData> allAatfs,
            DateTime currentDate,
            int complianceYear,
            ManageEvidenceNoteViewModel manageEvidenceViewModel)
        {
            var result = await client.SendAsync(User.GetAccessToken(), 
                new GetAatfNotesRequest(organisationId, aatfId, new List<NoteStatus> { NoteStatus.Draft, NoteStatus.Returned },
                manageEvidenceViewModel?.FilterViewModel.SearchRef, complianceYear, null, null, null, null, null));

            var model = mapper.Map<EditDraftReturnedNotesViewModel>(new EvidenceNotesViewModelTransfer(organisationId, aatfId, result, currentDate, manageEvidenceViewModel));

            model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>
                (new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, allAatfs, manageEvidenceViewModel?.FilterViewModel, null, null, complianceYear, currentDate));

            return this.View("Overview/EditDraftReturnedNotesOverview", model);
        }
    }
}