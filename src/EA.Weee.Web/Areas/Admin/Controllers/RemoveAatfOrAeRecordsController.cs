namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFOrAeRecords;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class RemoveAatfOrAeRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int pageSize = 10;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;

        public RemoveAatfOrAeRecordsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper, ConfigurationService configurationService)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
            this.configurationService = configurationService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(FacilityType facilityType, int page = 1, string name = null,
                                              string approvalNumber = null, int? selectedComplianceYear = null, int? selectedAatfStatus = null)
        {
            await SetBreadcrumb(facilityType);

            var complianceYearList = await GetRetentionPeriodComplianceYearList(facilityType);
            var statusList = GetAatfStatusList();
            int retentionPeriod = configurationService.CurrentConfiguration.RetentionPeriod;

            var removeAatfsViewModel = new RemoveAatfOrAeViewModel
            {
                ComplianceYearList = complianceYearList,
                AatfStatuses = statusList,
                SelectedComplianceYear = selectedComplianceYear,
                SelectedAatfStatus = selectedAatfStatus,
                Name = name,
                ApprovalNumber = approvalNumber,
                FacilityType = facilityType,
                RetentionPeriod = retentionPeriod
            };

            removeAatfsViewModel.AatfDataLists = await GetAatfDataListForRetentionPeriod(removeAatfsViewModel, page);

            return View(removeAatfsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RemoveAatfOrAeViewModel model, int pageNumber = 1)
        {
            await SetBreadcrumb(model.FacilityType);

            var aatfDataLists = await GetAatfDataListForRetentionPeriod(model, pageNumber);
            var complianceYearList = await GetRetentionPeriodComplianceYearList(model.FacilityType);
            var statusList = GetAatfStatusList();
            int retentionPeriod = configurationService.CurrentConfiguration.RetentionPeriod;

            model = new RemoveAatfOrAeViewModel
            {
                Name = model.Name,
                ApprovalNumber = model.ApprovalNumber,
                ComplianceYearList = complianceYearList,
                SelectedComplianceYear = model.SelectedComplianceYear,
                AatfStatuses = statusList,
                SelectedAatfStatus = model.SelectedAatfStatus,
                AatfDataLists = aatfDataLists,
                RetentionPeriod = retentionPeriod,
                FacilityType = model.FacilityType
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmDeletion(Guid aatfId, string name, string approvalNumber, int complianceYear, string status, FacilityType facilityType)
        {
            await SetBreadcrumb(facilityType);

            using (var client = apiClient())
            {
                var model = new RemoveAatfOrAeRecordConfirmViewModel
                {
                    AATFId = aatfId,
                    Name = name,
                    ApprovalNumber = approvalNumber,
                    ComplianceYear = complianceYear,
                    Status = status,
                    FacilityType = facilityType
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeletion(RemoveAatfOrAeRecordConfirmViewModel model)
        {
            using (var client = apiClient())
            {
                var request = new DeleteAatfOrAeRecordById(model.AATFId);
                var result = await client.SendAsync(User.GetAccessToken(), request);

                if (result)
                {
                    return RedirectToAction("Deleted", model);
                }
                else
                {
                    return RedirectToAction("DeleteFailure", model);
                }
            }
        }

        [HttpGet]
        public ActionResult Deleted(RemoveAatfOrAeRecordConfirmViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteFailure(RemoveAatfOrAeRecordConfirmViewModel model)
        {
            return View(model);
        }

        private async Task SetBreadcrumb(FacilityType facilityType)
        {
            if (facilityType == FacilityType.Aatf)
            {
                breadcrumb.InternalActivity = "Remove AATF records";
            }
            else
            {
                breadcrumb.InternalActivity = "Remove AE records";
            }

            await Task.Yield();
        }

        private async Task<List<int>> GetRetentionPeriodComplianceYearList(FacilityType facilityType)
        {
            using (var client = apiClient())
            {
                int retentionPeriod = configurationService.CurrentConfiguration.RetentionPeriod;
                var complianceYears = await client.SendAsync(User.GetAccessToken(),
                                                             new GetAatfOrAeRetentionPeriodComplianceYears(facilityType, retentionPeriod));

                return complianceYears;
            }
        }

        private List<AatfStatus> GetAatfStatusList()
        {
            return new List<AatfStatus>()
            {
                AatfStatus.Approved,
                AatfStatus.Suspended,
                AatfStatus.Cancelled
            };
        }

        private async Task<IPagedList<AatfDataList>> GetAatfDataListForRetentionPeriod(RemoveAatfOrAeViewModel model, int pageNumber = 1)
        {
            using (var client = apiClient())
            {
                var removeAatfOrAeFilter = new RemoveAatfOrAeFilter()
                {
                    Name = model.Name,
                    ApprovalNumber = model.ApprovalNumber,
                    ComplianceYear = model.SelectedComplianceYear,
                    SelectedStatus = model.SelectedAatfStatus,
                    UserId = User.GetUserId(),
                    FacilityType = model.FacilityType,
                    RetenctionPeriod = model.RetentionPeriod
                };

                var result = await client.SendAsync(User.GetAccessToken(), new GetAatfOrAeRetentionPeriodData(removeAatfOrAeFilter));

                return result.ToPagedList(pageNumber - 1, pageSize, result.Count);
            }
        }
    }
}