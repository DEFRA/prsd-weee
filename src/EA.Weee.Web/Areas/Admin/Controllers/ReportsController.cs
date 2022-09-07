namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfReturn;
    using Core.Admin;
    using Core.Shared;
    using EA.Weee.Core.Admin.AatfReports;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using Services;
    using ViewModels.AatfReports;
    using ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.AatfReports;

    public class ReportsController : ReportsBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public ReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb) : base(apiClient, breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        // GET: Admin/Reports
        public async Task<ActionResult> Index()
        {
            SetBreadcrumb();

            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        return RedirectToAction("ChooseReport", "Reports");
                    case UserStatus.Inactive:
                    case UserStatus.Pending:
                    case UserStatus.Rejected:
                        return RedirectToAction("InternalUserAuthorisationRequired", "Account", new { userStatus });
                    default:
                        throw new NotSupportedException(
                            $"Cannot determine result for user with status '{userStatus}'");
                }
            }
        }

        [HttpGet]
        public ActionResult ChooseReport()
        {
            SetBreadcrumb();

            var model = new ChooseReportTypeModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseReportTypeModel model)
        {
            SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.PcsReports:
                    return RedirectToAction("Index", "SchemeReports");

                case Reports.AatfReports:
                    return RedirectToAction("Index", "AatfReports");

                case Reports.PcsAatfDataDifference:
                    return RedirectToAction(nameof(PcsAatfDataDifference), "Reports");

                case Reports.AatfAeDetails:
                    return RedirectToAction(nameof(AatfAeDetails), "Reports");

                case Reports.EvidenceNotesReports:
                    return RedirectToAction("ChooseReport", "EvidenceReports");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> AatfAeDetails()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfAeDetailsViewModel();
            await PopulateFilters(model);

            return View("AatfAeDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfAeDetails(AatfAeDetailsViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfAeDetailsCsv(int complianceYear,
             ReportFacilityType? facilityType, Guid? authorityId, Guid? panAreaId, Guid? localAreaId)
        {
            CSVFileData fileData;

            var request = new GetAatfAeDetailsCsv(complianceYear, facilityType, authorityId, panAreaId, localAreaId, false);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> PcsAatfDataDifference()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new PcsAatfDataDifferenceViewModel();
            await PopulateFilters(model);

            return View("PcsAatfDataDifference", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PcsAatfDataDifference(PcsAatfDataDifferenceViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadPcsAatfDataDifference(int complianceYear, int? quarter,
            string obligationType)
        {
            CSVFileData fileData;

            var request = new GetPcsAatfComparisonData(complianceYear, quarter, obligationType);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        private async Task PopulateFilters(AatfAeDetailsViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatf());
            model.FacilityTypes = new SelectList(EnumHelper.GetValues(typeof(ReportFacilityType)), "Key", "Value");
            model.CompetentAuthoritiesList = await CompetentAuthoritiesList();
            model.PanAreaList = await PatAreaList();
            model.LocalAreaList = await LocalAreaList();
        }

        private async Task PopulateFilters(PcsAatfDataDifferenceViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatfReturns());
        }
    }
}