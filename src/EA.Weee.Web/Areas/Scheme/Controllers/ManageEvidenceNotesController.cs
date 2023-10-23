namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using Attributes;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Extensions;
    using Filters;
    using Prsd.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        private readonly ConfigurationService configurationService;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;

        public ManageEvidenceNotesController(IMapper mapper,
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            Func<IWeeeClient> apiClient,
            ISessionService sessionService,
            IMvcTemplateExecutor templateExecutor,
            IPdfDocumentProvider pdfDocumentProvider,
            ConfigurationService configurationService) : base(breadcrumb, cache)
        {
            this.mapper = mapper;
            this.apiClient = apiClient;
            this.sessionService = sessionService;
            this.templateExecutor = templateExecutor;
            this.pdfDocumentProvider = pdfDocumentProvider;
            this.configurationService = configurationService;
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> Index(Guid pcsId, string tab = null, int? selectedComplianceYear = null, int? page = 1, DateTime? startDate = null, DateTime? endDate = null, Guid? receivedId = null,
                                              int? wasteTypeValue = null, int? evidenceNoteTypeValue = null, string searchRef = null, int? noteStatusValue = null, Guid? submittedBy = null)
        {
            var manageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel()
            {
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

            return await ProcessManageEvidenceNotes(pcsId, tab, manageEvidenceNoteViewModel, page.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Guid pcsId, string tab = null, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel = null, int page = 1)
        {
            return await ProcessManageEvidenceNotes(pcsId, tab, manageEvidenceNoteViewModel, page);
        }

        private async Task<ActionResult> ProcessManageEvidenceNotes(Guid pcsId, string tab, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, int page)
        {
            sessionService.ClearTransferSessionObject(SessionKeyConstant.EditTransferTonnageViewModelKey);
            sessionService.ClearTransferSessionObject(SessionKeyConstant.TransferNoteKey);

            using (var client = this.apiClient())
            {
                var scheme = await Cache.FetchSchemePublicInfo(pcsId);

                await SetBreadcrumb(pcsId);

                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
                var selectedComplianceYear = SelectedComplianceYear(currentDate, manageEvidenceNoteViewModel);

                if (tab == null)
                {
                    tab = DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.Summary);
                }

                var value = tab.GetValueFromDisplayName<ManageEvidenceNotesDisplayOptions>();

                switch (value)
                {
                    case ManageEvidenceNotesDisplayOptions.Summary:
                        return await CreateAndPopulateEvidenceSummaryViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, selectedComplianceYear);

                    case ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence:
                        return await CreateAndPopulateReviewSubmittedEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);

                    case ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence:
                        return await CreateAndPopulateViewAndTransferEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);

                    case ManageEvidenceNotesDisplayOptions.OutgoingTransfers:
                        return await CreateAndPopulateOutgoingTransfersEvidenceViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, page, selectedComplianceYear);

                    default:
                        return await CreateAndPopulateEvidenceSummaryViewModel(pcsId, scheme, currentDate, manageEvidenceNoteViewModel, selectedComplianceYear);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(Guid organisationId, int complianceYear)
        {
            return RedirectToAction("TransferEvidenceNote", "TransferEvidence", new { pcsId = organisationId, complianceYear = complianceYear });
        }

        private async Task<ActionResult> CreateAndPopulateReviewSubmittedEvidenceViewModel(Guid organisationId, SchemePublicInfo scheme, DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, int pageNumber, int selectedComplianceYear)
        {
            var defaultNoteStatusList = new List<NoteStatus>() { NoteStatus.Submitted };
            var defaultNoteTypeList = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };
            var defaultWasteTypeList = new List<WasteType>() { WasteType.Household, WasteType.NonHousehold };

            using (var client = this.apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(),
                new GetEvidenceNotesByOrganisationRequest(
                    organisationId,
                    new List<NoteStatus>() { NoteStatus.Submitted },
                    selectedComplianceYear,
                    GetEvidenceNoteTypeQueryFilter(manageEvidenceNoteViewModel, defaultNoteTypeList),
                    false,
                    pageNumber,
                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize,
                    manageEvidenceNoteViewModel?.FilterViewModel.SearchRef,
                    manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy,
                    manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.StartDate,
                    manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.EndDate,
                    GetWasteTypeQueryFilter(manageEvidenceNoteViewModel, defaultWasteTypeList),
                    null));

                List<EntityIdDisplayNameData> submittedByFilterList = await GetSubmittedByList(organisationId, selectedComplianceYear, defaultNoteStatusList, defaultNoteTypeList, defaultWasteTypeList, client);

                var model = mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                    new SchemeTabViewModelMapTransfer(organisationId, result, scheme, currentDate, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>
                    (new ManageEvidenceNoteTransfer(organisationId, manageEvidenceNoteViewModel?.FilterViewModel, null, null, selectedComplianceYear, currentDate));

                model.ManageEvidenceNoteViewModel.SubmittedDatesFilterViewModel = mapper.Map<SubmittedDatesFilterViewModel>(
                    new SubmittedDateFilterBase(manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.StartDate, manageEvidenceNoteViewModel?.SubmittedDatesFilterViewModel.EndDate));

                model.ManageEvidenceNoteViewModel.RecipientWasteStatusFilterViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(
                            new RecipientWasteStatusFilterBase(null,
                            null,
                            manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                            manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue,
                            manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy,
                            submittedByFilterList,
                            manageEvidenceNoteViewModel?.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue,
                            false,
                            true));

                return View("ReviewSubmittedEvidence", model);
            }
        }

        private async Task<ActionResult> CreateAndPopulateViewAndTransferEvidenceViewModel(Guid organisationId, SchemePublicInfo scheme, DateTime currentDate, ManageEvidenceNoteViewModel noteViewModel, int pageNumber, int selectedComplianceYear)
        {
            var defaultNoteStatusList = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Void, NoteStatus.Returned, NoteStatus.Draft, NoteStatus.Submitted };
            var defaultNoteTypeList = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };
            var defaultWasteTypeList = new List<WasteType>() { WasteType.Household, WasteType.NonHousehold };

            using (var client = this.apiClient())
            {
                List<NoteStatus> noteStatusQueryFilterList = GetNoteStatusQueryFilter(noteViewModel, defaultNoteStatusList);
                List<NoteType> noteTypeQueryFilterList = GetEvidenceNoteTypeQueryFilter(noteViewModel, defaultNoteTypeList);
                List<WasteType> wasteTypeQueryFilterList = GetWasteTypeQueryFilter(noteViewModel, defaultWasteTypeList);

                var result = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNotesByOrganisationRequest(organisationId,
                                                    noteStatusQueryFilterList, selectedComplianceYear, noteTypeQueryFilterList, false, pageNumber,
                                                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize,
                                                    noteViewModel?.FilterViewModel.SearchRef,
                                                    noteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy,
                                                    noteViewModel?.SubmittedDatesFilterViewModel.StartDate,
                                                    noteViewModel?.SubmittedDatesFilterViewModel.EndDate,
                                                    wasteTypeQueryFilterList,
                                                    null));

                List<EntityIdDisplayNameData> submittedByFilterList = await GetSubmittedByList(organisationId, selectedComplianceYear, defaultNoteStatusList, defaultNoteTypeList, defaultWasteTypeList, client);

                var submittedDatesFilterViewModel = mapper.Map<SubmittedDatesFilterViewModel>(new SubmittedDateFilterBase(noteViewModel?.SubmittedDatesFilterViewModel.StartDate, noteViewModel?.SubmittedDatesFilterViewModel.EndDate));
                var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(new RecipientWasteStatusFilterBase(null, null, noteViewModel?.RecipientWasteStatusFilterViewModel.WasteTypeValue,
                                                                                                                                       noteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue, noteViewModel?.RecipientWasteStatusFilterViewModel.SubmittedBy,
                                                                                                                                       submittedByFilterList, noteViewModel?.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue, true, true));

                var model = mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(new SchemeTabViewModelMapTransfer(organisationId, result, scheme, currentDate, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));

                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, noteViewModel?.FilterViewModel, recipientWasteStatusViewModel, submittedDatesFilterViewModel, selectedComplianceYear, currentDate));

                return View("ViewAndTransferEvidence", model);
            }
        }

        private async Task<List<EntityIdDisplayNameData>> GetSubmittedByList(Guid organisationId, int selectedComplianceYear, List<NoteStatus> defaultNoteStatusList, List<NoteType> defaultNoteTypeList, List<WasteType> defaultWasteTypeList, IWeeeClient client)
        {
            var aatfResults = await client.SendAsync(User.GetAccessToken(),
                                new GetEvidenceNotesByRequest(organisationId,
                                    defaultNoteStatusList,
                                    selectedComplianceYear,
                                    defaultNoteTypeList,
                                    false,
                                    defaultWasteTypeList,
                                    1, int.MaxValue));

            var submittedByFilterList = new List<EntityIdDisplayNameData>();
            foreach (var aatfRecord in aatfResults.Results)
            {
                (Guid Id, string Name) submittedBy = GetSubmittedBy(aatfRecord);
                if (!submittedByFilterList.Any(x => x.DisplayName == submittedBy.Name) && !string.IsNullOrWhiteSpace(submittedBy.Name))
                {
                    submittedByFilterList.Add(new EntityIdDisplayNameData()
                    {
                        Id = submittedBy.Id,
                        DisplayName = submittedBy.Name
                    });
                }
            }

            return submittedByFilterList.OrderBy(x => x.DisplayName).ToList();
        }

        private static (Guid, string) GetSubmittedBy(EvidenceNoteData aatfResult)
        {
            if (aatfResult.Type == NoteType.Transfer)
            {
                if (aatfResult.OrganisationSchemaData != null)
                {
                    return (aatfResult.OrganisationSchemaData.Id, aatfResult.OrganisationSchemaData.SchemeName);
                }
                else
                {
                    return (aatfResult.OrganisationData.Id, aatfResult.OrganisationData.OrganisationName);
                }
            }
            else if (aatfResult.SubmittedDate.HasValue)
            {
                return (aatfResult.AatfData.Id, aatfResult.AatfData.Name);
            }
            else
            {
                return (Guid.Empty, null);
            }
        }

        private static List<NoteStatus> GetNoteStatusQueryFilter(ManageEvidenceNoteViewModel noteViewModel, List<NoteStatus> defaultNoteStatusList)
        {
            //if there's a selected NoteStatus then use that for the filter, otherwise all
            var noteStatusQueryFilterList = new List<NoteStatus>();
            if (noteViewModel.RecipientWasteStatusFilterViewModel.NoteStatusValue != null)
            {
                noteStatusQueryFilterList.Add((NoteStatus)noteViewModel.RecipientWasteStatusFilterViewModel.NoteStatusValue);
            }
            else
            {
                noteStatusQueryFilterList.AddRange(defaultNoteStatusList);
            }

            return noteStatusQueryFilterList;
        }

        private static List<WasteType> GetWasteTypeQueryFilter(ManageEvidenceNoteViewModel noteViewModel, List<WasteType> defaultWasteTypeList)
        {
            var wasteTypeList = new List<WasteType>();
            if (noteViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue != null)
            {
                wasteTypeList.Add((WasteType)noteViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue);
            }
            else
            {
                wasteTypeList.AddRange(defaultWasteTypeList);
            }

            return wasteTypeList;
        }

        private static List<NoteType> GetEvidenceNoteTypeQueryFilter(ManageEvidenceNoteViewModel noteViewModel, List<NoteType> defaultNoteTypeList)
        {
            var noteTypeList = new List<NoteType>();
            if (noteViewModel.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue != null)
            {
                noteTypeList.Add((NoteType)noteViewModel.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue);
            }
            else
            {
                noteTypeList.AddRange(defaultNoteTypeList);
            }

            return noteTypeList;
        }

        private async Task<ActionResult> CreateAndPopulateOutgoingTransfersEvidenceViewModel(Guid organisationId, SchemePublicInfo scheme, DateTime currentDate, ManageEvidenceNoteViewModel noteViewModel, int pageNumber, int selectedComplianceYear)
        {
            using (var client = this.apiClient())
            {
                var noteStatusList = new List<NoteStatus>();
                if (noteViewModel.RecipientWasteStatusFilterViewModel.NoteStatusValue == null)
                {
                    noteStatusList.AddRange(new List<NoteStatus>()
                    {
                        NoteStatus.Draft,
                        NoteStatus.Approved,
                        NoteStatus.Rejected,
                        NoteStatus.Submitted,
                        NoteStatus.Void,
                        NoteStatus.Returned
                    });
                }
                else
                {
                    noteStatusList.Add((NoteStatus)noteViewModel.RecipientWasteStatusFilterViewModel.NoteStatusValue);
                }

                var noteTypeList = new List<NoteType>();
                if (noteViewModel.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue == null)
                {
                    noteTypeList.AddRange(new List<NoteType>() { NoteType.Transfer });
                }
                else
                {
                    noteTypeList.Add((NoteType)noteViewModel.RecipientWasteStatusFilterViewModel.EvidenceNoteTypeValue);
                }

                var wasteTypeList = new List<WasteType>();
                if (noteViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue == null)
                {
                    wasteTypeList.AddRange(new List<WasteType>() { WasteType.Household, WasteType.NonHousehold });
                }
                else
                {
                    wasteTypeList.Add((WasteType)noteViewModel.RecipientWasteStatusFilterViewModel.WasteTypeValue);
                }

                Guid? submittedById = null;
                if (noteViewModel.RecipientWasteStatusFilterViewModel.SubmittedBy.HasValue)
                {
                    submittedById = noteViewModel.RecipientWasteStatusFilterViewModel.SubmittedBy.Value;
                }

                Guid? receivedId = null;
                if (noteViewModel.RecipientWasteStatusFilterViewModel.ReceivedId.HasValue)
                {
                    receivedId = noteViewModel.RecipientWasteStatusFilterViewModel.ReceivedId.Value;
                }

                var result = await client.SendAsync(User.GetAccessToken(),
                                                    new GetEvidenceNotesByOrganisationRequest(organisationId, noteStatusList, selectedComplianceYear, noteTypeList,
                                                    true,
                                                    pageNumber,
                                                    configurationService.CurrentConfiguration.DefaultExternalPagingPageSize,
                                                    noteViewModel?.FilterViewModel.SearchRef,
                                                    (submittedById.HasValue ? submittedById.Value : (Guid?)null),
                                                    noteViewModel?.SubmittedDatesFilterViewModel.StartDate,
                                                    noteViewModel?.SubmittedDatesFilterViewModel.EndDate,
                                                    wasteTypeList, receivedId));

                var submittedDatesFilterViewModel = mapper.Map<SubmittedDatesFilterViewModel>(new SubmittedDateFilterBase(noteViewModel?.SubmittedDatesFilterViewModel.StartDate, noteViewModel?.SubmittedDatesFilterViewModel.EndDate));

                var defaultStatusList = new List<NoteStatus>() { NoteStatus.Draft, NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Returned };
                var defaultNoteTypeList = new List<NoteType>() { NoteType.Transfer };
                var defaultWasteTypeList = new List<WasteType>() { WasteType.Household, WasteType.NonHousehold };

                var aatfResults = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNotesByRequest(organisationId, defaultStatusList, selectedComplianceYear, defaultNoteTypeList, true, defaultWasteTypeList, 1, int.MaxValue));

                var recipientData = new List<EntityIdDisplayNameData>();
                for (int count = 0; count < aatfResults.Results.Count(); count++)
                {
                    if (aatfResults.Results[count].RecipientOrganisationData != null)
                    {
                        var isValueAvailable = recipientData.Find(x => x.DisplayName == aatfResults.Results[count].RecipientSchemeData?.SchemeName);
                        if (isValueAvailable == null)
                        {
                            recipientData.Add(new EntityIdDisplayNameData()
                            {
                                Id = aatfResults.Results[count].RecipientId,
                                DisplayName = aatfResults.Results[count].RecipientSchemeData.SchemeName
                            });
                        }
                    }
                }

                var recipientWasteStatusViewModel = mapper.Map<RecipientWasteStatusFilterViewModel>(new RecipientWasteStatusFilterBase(recipientData, noteViewModel?.RecipientWasteStatusFilterViewModel.ReceivedId, null,
                                                                                                                                       noteViewModel?.RecipientWasteStatusFilterViewModel.NoteStatusValue, null, null, null, false, true));
                var model = mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(new SchemeTabViewModelMapTransfer(organisationId, result, scheme, currentDate, selectedComplianceYear, pageNumber, configurationService.CurrentConfiguration.DefaultExternalPagingPageSize));
                model.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(organisationId, noteViewModel?.FilterViewModel, recipientWasteStatusViewModel, submittedDatesFilterViewModel, selectedComplianceYear, currentDate));

                return View("OutgoingTransfers", model);
            }
        }

        [HttpGet]
        [CheckCanApproveNote]
        [NoCacheFilter]
        public async Task<ActionResult> ReviewEvidenceNote(Guid pcsId, Guid evidenceNoteId, string queryString = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId);

                ReviewEvidenceNoteViewModel model = await GetNote(pcsId, evidenceNoteId, queryString, client);

                if (model.ViewEvidenceNoteViewModel.Status != NoteStatus.Submitted)
                {
                    return RedirectToAction("Index", "ManageEvidenceNotes", new { pcsId, @tab = ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString() });
                }

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

                    var request = new SetNoteStatusRequest(model.ViewEvidenceNoteViewModel.Id, status, model.Reason);

                    TempData[ViewDataConstant.EvidenceNoteStatus] = (NoteUpdatedStatusEnum)request.Status;

                    await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("ViewEvidenceNote",
                        new { organisationId = model.OrganisationId, evidenceNoteId = request.NoteId, selectedComplianceYear = model.ViewEvidenceNoteViewModel.ComplianceYear });
                }

                await SetBreadcrumb(model.OrganisationId);

                model = await GetNote(model.ViewEvidenceNoteViewModel.SchemeId, model.ViewEvidenceNoteViewModel.Id, model.QueryString, client);

                return View("ReviewEvidenceNote", model);
            }
        }

        [HttpGet]
        [NoCacheFilter]
        public async Task<ActionResult> ViewEvidenceNote(Guid pcsId, Guid evidenceNoteId,
            string redirectTab = null,
            int page = 1,
            bool openedInNewTab = false,
            string queryString = null)
        {
            using (var client = this.apiClient())
            {
                await SetBreadcrumb(pcsId);

                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, TempData[ViewDataConstant.EvidenceNoteStatus], false)
                {
                    SchemeId = pcsId,
                    RedirectTab = redirectTab,
                    OpenedInNewTab = openedInNewTab,
                    QueryString = queryString
                });

                ViewBag.Page = page;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndPopulateEvidenceSummaryViewModel(Guid pcsId, SchemePublicInfo scheme, DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, int selectedComplianceYear)
        {
            using (var client = apiClient())
            {
                GetObligationSummaryRequest request = null;
                if (scheme.IsBalancingScheme)
                {
                    // PBS do not have a scheme id - we send a null to the Stored Proc which will use organisation id instead
                    request = new GetObligationSummaryRequest(null, pcsId, selectedComplianceYear);

                    var evidenceSummaryData = await client.SendAsync(User.GetAccessToken(), request);

                    var pbsSummaryModel = mapper.Map<SummaryEvidenceViewModel>
                        (new ViewEvidenceSummaryViewModelMapTransfer(pcsId, evidenceSummaryData, manageEvidenceNoteViewModel, scheme, currentDate, selectedComplianceYear));

                    pbsSummaryModel.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, null, null, selectedComplianceYear, currentDate));

                    return View("SummaryEvidencePBS", pbsSummaryModel);
                }

                // used by Scheme users
                request = new GetObligationSummaryRequest(scheme.SchemeId, pcsId, selectedComplianceYear);

                var obligationEvidenceSummaryData = await client.SendAsync(User.GetAccessToken(), request);

                var summaryModel = mapper.Map<SummaryEvidenceViewModel>
                    (new ViewEvidenceSummaryViewModelMapTransfer(pcsId, obligationEvidenceSummaryData, manageEvidenceNoteViewModel, scheme, currentDate, selectedComplianceYear));

                summaryModel.ManageEvidenceNoteViewModel = mapper.Map<ManageEvidenceNoteViewModel>(new ManageEvidenceNoteTransfer(pcsId, null, null, null, selectedComplianceYear, currentDate));

                return View("SummaryEvidence", summaryModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNotePdf(Guid evidenceNoteId)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteForSchemeRequest(evidenceNoteId);

                var result = await client.SendAsync(User.GetAccessToken(), request);

                var model = mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null, true) { PrintableVersion = true });

                var content = templateExecutor.RenderRazorView(ControllerContext, "DownloadEvidenceNotePdf", model);

                var pdf = pdfDocumentProvider.GeneratePdfFromHtml(content);

                var timestamp = SystemTime.Now;
                var fileName = $"{result.ComplianceYear}_{model.ReferenceDisplay}{timestamp.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}.pdf";

                return File(pdf, "application/pdf", fileName);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNotesReport(Guid pcsId, int complianceYear, TonnageToDisplayReportEnum tonnageToDisplay)
        {
            using (var client = apiClient())
            {
                var request = new GetEvidenceNoteReportRequest(pcsId, null, tonnageToDisplay, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceTransfersReport(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetTransferNoteReportRequest(complianceYear, pcsId);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceSummaryReport(Guid pcsId, int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetSchemeObligationAndEvidenceTotalsReportRequest(null, null, pcsId, complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(file.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
            }
        }

        private int SelectedComplianceYear(DateTime currentDate, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            return ComplianceYearHelper.GetSelectedComplianceYear(manageEvidenceNoteViewModel, currentDate);
        }

        private async Task<ReviewEvidenceNoteViewModel> GetNote(Guid pcsId, Guid evidenceNoteId, string queryString, IWeeeClient client)
        {
            var result = await client.SendAsync(User.GetAccessToken(), new GetEvidenceNoteForSchemeRequest(evidenceNoteId));

            var model = mapper.Map<ReviewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(result, null, false)
            {
                SchemeId = pcsId,
                QueryString = queryString
            });

            return model;
        }
    }
}