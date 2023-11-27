namespace EA.Weee.Web.Areas.Aatf.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AatfEvidence.Controllers;
    using Api.Client;
    using Attributes;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Constants;
    using Core.Helpers;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Constant;
    using Extensions;
    using Filters;
    using Infrastructure;
    using Infrastructure.PDF;
    using Mappings.ToViewModel;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Prsd.Core.Web.ApiClient;
    using Services;
    using Services.Caching;
    using ViewModels;
    using Web.Requests.Base;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
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
        private readonly ConfigurationService configurationService;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient,
            IRequestCreator<EditEvidenceNoteViewModel, CreateEvidenceNoteRequest> createRequestCreator,
            IRequestCreator<EditEvidenceNoteViewModel, EditEvidenceNoteRequest> editRequestCreator,
            ISessionService sessionService,
            ConfigurationService configurationService,
            IMvcTemplateExecutor templateExecutor, IPdfDocumentProvider pdfDocumentProvider)
        {
            this.mapper = mapper;
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.apiClient = apiClient;
            this.createRequestCreator = createRequestCreator;
            this.editRequestCreator = editRequestCreator;
            this.sessionService = sessionService;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.configurationService = configurationService;
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> Index(Guid organisationId, Guid aatfId, string tab = null, int? selectedComplianceYear = null, int? page = 1, DateTime? startDate = null,
                                              DateTime? endDate = null, Guid? receivedId = null, int? wasteTypeValue = null, int? evidenceNoteTypeValue = null, string searchRef = null,
                                              int? noteStatusValue = null, Guid? submittedBy = null)
        {
            var manageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel()
            {
                AatfId = aatfId,
                SelectedComplianceYear = selectedComplianceYear.HasValue ? selectedComplianceYear.Value : 0,
                RecipientWasteStatusFilterViewModel = new RecipientWasteStatusFilterViewModel()
                {
                    NoteStatusValue = (noteStatusValue.HasValue ? (NoteStatus)noteStatusValue : (NoteStatus?)null),
                    ReceivedId = (receivedId.HasValue ? receivedId : null),
                    SubmittedBy = (submittedBy.HasValue ? submittedBy : null),
                    WasteTypeValue = (wasteTypeValue.HasValue ? (WasteType)wasteTypeValue : (WasteType?)null),
                    EvidenceNoteTypeValue = (evidenceNoteTypeValue.HasValue ? (EvidenceNoteType)evidenceNoteTypeValue : (EvidenceNoteType?)null)
                },
                FilterViewModel = new FilterViewModel()
                {
                    SearchRef = searchRef
                },
                SubmittedDatesFilterViewModel = new SubmittedDatesFilterViewModel()
                {
                    StartDate = startDate,
                    EndDate = endDate
                }
            };
            page = page ?? 1;
            return await ProcessManageEvidenceNotes(organisationId, tab, manageEvidenceNoteViewModel, page.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Guid organisationId, string tab = null, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null, int page = 1)
        {
            return await ProcessManageEvidenceNotes(organisationId, tab, manageEvidenceNoteViewModel, page);
        }

        private async Task<ActionResult> ProcessManageEvidenceNotes(Guid organisationId, string tab, ManageEvidenceNoteViewModel evidenceNoteViewModel, int page)
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
                int selectedComplianceYear = SelectedComplianceYear(currentDate, evidenceNoteViewModel);

                var aatf = await client.SendAsync(this.User.GetAccessToken(), new GetAatfByIdExternal(evidenceNoteViewModel.AatfId));
                var allAatfsAndAes = await cache.FetchAatfDataForOrganisationData(organisationId);

                switch (value)
                {
                    case ManageEvidenceOverviewDisplayOption.EvidenceSummary:
                        return await EvidenceSummaryCase(organisationId, evidenceNoteViewModel.AatfId, client, aatf, allAatfsAndAes, selectedComplianceYear, currentDate);

                    case ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes:
                        return await EditDraftReturnNoteCase(client, organisationId, evidenceNoteViewModel.AatfId, aatf, allAatfsAndAes, currentDate, selectedComplianceYear, evidenceNoteViewModel, page);

                    case ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes:
                        return await ViewAllOtherEvidenceNotesCase(organisationId, evidenceNoteViewModel.AatfId, client, aatf, allAatfsAndAes, currentDate, selectedComplianceYear, evidenceNoteViewModel, page);

                    default:
                        return await EvidenceSummaryCase(organisationId, evidenceNoteViewModel.AatfId, client, aatf, allAatfsAndAes, selectedComplianceYear, currentDate);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewEvidenceNote(ManageEvidenceNoteViewModel model)
        {
            return RedirectToAction("CreateEvidenceNote", "ManageEvidenceNotes", new { area = "Aatf", model.OrganisationId, model.AatfId, complianceYear = model.SelectedComplianceYear });
        }

        [HttpGet]
        [CheckCanCreateEvidenceNote]
        [NoCacheFilter]

        public async Task<ActionResult> CreateEvidenceNote(Guid organisationId, Guid aatfId, int complianceYear, bool returnFromCopyPaste = false)
        {
            using (var client = apiClient())
            {
                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));

                var existingModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(SessionKeyConstant.EditEvidenceViewModelKey);

                sessionService.SetTransferSessionObject(null, SessionKeyConstant.EditEvidenceViewModelKey);

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
                    sessionService.SetTransferSessionObject(viewModel, SessionKeyConstant.EditEvidenceViewModelKey);
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
        [NoCacheFilter]
        public async Task<ActionResult> ViewDraftEvidenceNote(Guid organisationId, Guid evidenceNoteId, int page = 1, string queryString = null)
        {
            using (var client = apiClient())
            {
                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                var request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus], false)
                {
                    QueryString = queryString
                });

                ViewBag.Page = page;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNote(Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null, true));

                var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadEvidenceNote", model);

                var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

                var timestamp = SystemTime.Now;
                var fileName = $"{result.ComplianceYear}_{model.ReferenceDisplay}{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.pdf";

                return File(pdf, "application/pdf", fileName);
            }
        }

        [HttpGet]
        [CheckCanEditEvidenceNote]
        [NoCacheFilter]
        public async Task<ActionResult> EditEvidenceNote(Guid organisationId, Guid evidenceNoteId, bool returnFromCopyPaste = false, string queryString = null, bool returnToView = false)
        {
            using (var client = apiClient())
            {
                var organisationSchemes = await client.SendAsync(User.GetAccessToken(), new GetOrganisationScheme(true));

                var existingModel = sessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(SessionKeyConstant.EditEvidenceViewModelKey);
                sessionService.SetTransferSessionObject(null, SessionKeyConstant.EditEvidenceViewModelKey);

                var request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);
                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = !returnFromCopyPaste ?
                    mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, null, organisationId, result.AatfData.Id, result, result.ComplianceYear, queryString, returnToView))
                    : mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, existingModel, organisationId, result.AatfData.Id, result, result.ComplianceYear, existingModel.QueryString, existingModel.ReturnToView));

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
                    sessionService.SetTransferSessionObject(viewModel, SessionKeyConstant.EditEvidenceViewModelKey);
                    return RedirectToAction("Index", EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName,
                        new { organisationId, returnAction = EvidenceCopyPasteActionConstants.EditEvidenceNoteAction, complianceYear = viewModel.ComplianceYear });
                }
                if (ModelState.IsValid)
                {
                    var request = editRequestCreator.ViewModelToRequest(viewModel);

                    NoteUpdatedStatusEnum updateStatus;
                    if (viewModel.Status == NoteStatus.Returned)
                    {
                        updateStatus = viewModel.Action == ActionEnum.Save ? NoteUpdatedStatusEnum.ReturnedSaved : NoteUpdatedStatusEnum.ReturnedSubmitted;
                    }
                    else
                    {
                        updateStatus = (NoteUpdatedStatusEnum)request.Status;
                    }

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

                var model = mapper.Map<EditEvidenceNoteViewModel>(new EditNoteMapTransfer(organisationSchemes, viewModel, organisationId, aatfId, null, viewModel.ComplianceYear, viewModel.QueryString, viewModel.ReturnToView));

                await SetBreadcrumb(organisationId, BreadCrumbConstant.AatfManageEvidence);

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> CancelEvidenceNote(Guid organisationId, Guid aatfId, Guid evidenceNoteId)
        {
            using (var client = this.apiClient())
            {
                var request = new SetNoteStatusRequest(evidenceNoteId, NoteStatus.Cancelled);

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
                //await client.SendAsync(User.GetAccessToken(), request);

                return RedirectToAction("ViewDraftEvidenceNote", new { organisationId = organisationId, evidenceNoteId = request.NoteId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceSummaryReport(Guid aatfId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetAatfSummaryReportRequest(aatfId, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNotesReport(Guid? aatfId, int complianceYear, TonnageToDisplayReportEnum tonnageToDisplay)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteReportRequest(null, aatfId, tonnageToDisplay, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
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
            var routeName = string.Empty;
            switch (status)
            {
                case NoteStatus.Draft:
                    routeName = AatfEvidenceRedirect.ViewDraftEvidenceRouteName;
                    break;

                case NoteStatus.Submitted:
                    routeName = AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;
                    break;

                case NoteStatus.Cancelled:
                    routeName = AatfEvidenceRedirect.ViewCancelEvidenceRouteName;
                    break;

                default:
                    break;
            }

            //var routeName = status == NoteStatus.Draft
            //    ? AatfEvidenceRedirect.ViewDraftEvidenceRouteName
            //    : AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName;

            return RedirectToRoute(routeName, new
            {
                organisationId,
                aatfId,
                evidenceNoteId = result
            });
        }

        private static List<WasteType> GetWasteTypeQueryFilter(ManageEvidenceNoteViewModel noteViewModel, List<WasteType> defaultWasteTypeList)
        {
            var wasteTypeList = new List<WasteType>();
            if (noteViewModel?.RecipientWasteStatusFilterViewModel?.WasteTypeValue != null)
            {
                wasteTypeList.Add((WasteType)noteViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue);
            }
            else
            {
                wasteTypeList.AddRange(defaultWasteTypeList);
            }

            return wasteTypeList;
        }

        private async Task<ActionResult> ViewAllOtherEvidenceNotesCase(Guid organisationId, Guid aatfId, IWeeeClient client, AatfData aatf,
            List<AatfData> allAatfs, DateTime currentDate, int selectedComplianceYear, ManageEvidenceNoteViewModel manageEvidenceViewModel, int pageNumber)
        {
            EvidenceNoteSearchDataResult resultAllNotes = new EvidenceNoteSearchDataResult();

            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            var defaultWasteTypeList = new List<WasteType>() { WasteType.Household, WasteType.NonHousehold };
            var defaultNoteTypes = new List<NoteType>() { NoteType.Evidence };

            if (ModelState.IsValid)
            {
                resultAllNotes = await client.SendAsync(User.GetAccessToken(), new GetAatfNotesRequest(organisationId, aatfId, allowedStatus,
                manageEvidenceViewModel?.FilterViewModel.SearchRef,
                selectedComplianceYear,
                manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                GetWasteTypeQueryFilter(manageEvidenceViewModel, defaultWasteTypeList),
                manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                manageEvidenceViewModel?.SubmittedDatesFilterViewModel.StartDate,
                manageEvidenceViewModel?.SubmittedDatesFilterViewModel.EndDate,
                pageNumber,
                configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));
            }

            var modelAllNotes = mapper.Map<AllOtherManageEvidenceNotesViewModel>(
                new EvidenceNotesViewModelTransfer(organisationId, aatfId, resultAllNotes, currentDate, manageEvidenceViewModel, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

            var schemeData = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNotesRecipientList(organisationId, aatfId, selectedComplianceYear, allowedStatus, defaultNoteTypes));

            var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                        new RecipientWasteStatusFilterBase(schemeData, manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue, null, new List<EntityIdDisplayNameData>(), null, false, false));

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
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(manageEvidenceNoteViewModel, currentDate);

            return complianceYear;
        }

        private async Task<ActionResult> EditDraftReturnNoteCase(IWeeeClient client, Guid organisationId, Guid aatfId, AatfData aatf, List<AatfData> allAatfs, DateTime currentDate,
                                                                 int complianceYear, ManageEvidenceNoteViewModel manageEvidenceViewModel, int pageNumber)
        {
            var defaultNoteStatus = new List<NoteStatus> { NoteStatus.Draft, NoteStatus.Returned };
            var recipientId = (Guid?)null;

            if (manageEvidenceViewModel.RecipientWasteStatusFilterViewModel.ReceivedId.HasValue)
            {
                recipientId = manageEvidenceViewModel.RecipientWasteStatusFilterViewModel.ReceivedId.Value;
            }

            var selectedWasteType = new List<WasteType>();
            if (manageEvidenceViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue.HasValue)
            {
                selectedWasteType.Add(manageEvidenceViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue.Value);
            }

            var result = await client.SendAsync(User.GetAccessToken(), new GetAatfNotesRequest(organisationId, aatfId, defaultNoteStatus, manageEvidenceViewModel?.FilterViewModel.SearchRef,
                                                                                               complianceYear, recipientId, selectedWasteType, manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel?.NoteStatusValue,
                                                                                               null, null, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

            var aatfResults = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNotesRecipientList(organisationId, aatfId, complianceYear, defaultNoteStatus, new List<NoteType> { NoteType.Evidence }));

            var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                        new RecipientWasteStatusFilterBase(aatfResults, manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId,
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                        manageEvidenceViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy, null, null, false, false, defaultNoteStatus));

            var model = mapper.Map<EditDraftReturnedNotesViewModel>(
                new EvidenceNotesViewModelTransfer(organisationId, aatfId, result, currentDate, manageEvidenceViewModel, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

            model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>
                (new ManageEvidenceNoteTransfer(organisationId, aatfId, aatf, allAatfs, manageEvidenceViewModel?.FilterViewModel, recipientWasteStatusViewModel, null, complianceYear, currentDate));

            return this.View("Overview/EditDraftReturnedNotesOverview", model);
        }
    }
}