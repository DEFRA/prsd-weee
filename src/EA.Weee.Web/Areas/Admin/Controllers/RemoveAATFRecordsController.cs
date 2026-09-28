namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared.Paging;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Requests.Admin.RemoveAATFRecords;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFRecords;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class RemoveAATFRecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int pageSize = 10;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public RemoveAATFRecordsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> Index(int page = 1, string name = null, string approvalNumber = null,
                                              int? selectedComplianceYear = null, int? selectedAatfStatus = null)
        {
            await SetBreadcrumb();

            var removeAatfsViewModel = new RemoveAatfsViewModel
            {
                ComplianceYearList = await GetAATFRetentionPeriodComplianceYears(),
                AatfStatuses = GetAatfStatusList(),
                SelectedComplianceYear = selectedComplianceYear,
                SelectedAatfStatus = selectedAatfStatus,
                Name = name,
                ApprovalNumber = approvalNumber
            };

            removeAatfsViewModel.AatfDataLists = await GetAatfDataListForRetentionPeriod(removeAatfsViewModel, page);

            return View(removeAatfsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> Index(RemoveAatfsViewModel model, int pageNumber = 1)
        {
            await SetBreadcrumb();

            var aatfDataLists = await GetAatfDataListForRetentionPeriod(model, pageNumber);

            model = new RemoveAatfsViewModel
            {
                Name = model.Name,
                ApprovalNumber = model.ApprovalNumber,
                ComplianceYearList = await GetAATFRetentionPeriodComplianceYears(),
                SelectedComplianceYear = model.SelectedComplianceYear,
                AatfStatuses = GetAatfStatusList(),
                SelectedAatfStatus = model.SelectedAatfStatus,
                AatfDataLists = aatfDataLists
            };

            return View(model);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ConfirmDeletion(Guid aatfId, string name, string approvalNumber, int complianceYear, string status)
        {
            await SetBreadcrumb();

            using (var client = apiClient())
            {
                var model = new RemoveAATFRecordConfirmViewModel
                {
                    AATFId = aatfId,
                    Name = name,
                    ApprovalNumber = approvalNumber,
                    ComplianceYear = complianceYear,
                    Status = status
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public async Task<ActionResult> ConfirmDeletion(RemoveAATFRecordConfirmViewModel model)
        {
            using (var client = apiClient())
            {
                var request = new DeleteAnAatfById(model.AATFId);
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
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public ActionResult Deleted(RemoveAATFRecordConfirmViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        [AuthorizeInternalClaims(Claims.InternalAdmin)]
        public ActionResult DeleteFailure(RemoveAATFRecordConfirmViewModel model)
        {
            return View(model);
        }

        private async Task SetBreadcrumb()
        {
            breadcrumb.InternalActivity = "Remove AATF records";

            await Task.Yield();
        }

        private async Task<List<int>> GetAATFRetentionPeriodComplianceYears()
        {
            using (var client = apiClient())
            {
                var complianceYears = await client.SendAsync(User.GetAccessToken(), new GetAatfRetentionPeriodComplianceYears());

                return complianceYears;
            }
        }

        private List<AatfStatus> GetAatfStatusList()
        {
            return new List<AatfStatus>() { AatfStatus.Approved, AatfStatus.Suspended, AatfStatus.Cancelled };
        }

        private async Task<IPagedList<AatfDataList>> GetAatfDataListForRetentionPeriod(RemoveAatfsViewModel model, int pageNumber = 1)
        {
            using (var client = apiClient())
            {
                var removeaatfFilter = new RemoveAATFFilter()
                {
                    Name = model.Name,
                    ApprovalNumber = model.ApprovalNumber,
                    ComplianceYear = model.SelectedComplianceYear,
                    SelectedStatus = model.SelectedAatfStatus,
                    UserId = User.GetUserId()
                };

                var result = await client.SendAsync(User.GetAccessToken(), new GetAatfRetentionPeriodDataList(removeaatfFilter));

                return result.ToPagedList(pageNumber - 1, pageSize, result.Count);
            }
        }
    }
}