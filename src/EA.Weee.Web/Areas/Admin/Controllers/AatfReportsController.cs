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
    using EA.Weee.Requests.Admin.AatfReports;
    using EA.Weee.Web.Areas.Admin.ViewModels.AatfReports;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using Services;
    using ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Shared;

    public class AatfReportsController : ReportsBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public AatfReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb) : base(apiClient, breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
        }

        public async Task<ActionResult> Index()
        {
            SetBreadcrumb();

            return await CheckUserStatus("AatfReports", "ChooseReport");
        }

        [HttpGet]
        public ActionResult ChooseReport()
        {
            SetBreadcrumb();

            var model = new ChooseAatfReportViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseAatfReportViewModel model)
        {
            SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.AatfAeReturnData:
                    return RedirectToAction(nameof(AatfAeReturnData), "AatfReports");

                case Reports.AatfObligatedData:
                    return RedirectToAction(nameof(AatfObligatedData), "AatfReports");

                case Reports.UkNonObligatedWeeeData:
                    return RedirectToAction(nameof(UkNonObligatedWeeeReceived), "AatfReports");

                case Reports.AatfNonObligatedData:
                    return RedirectToAction(nameof(AatfNonObligatedData), "AatfReports");

                case Reports.AatfSentOnData:
                    return RedirectToAction(nameof(AatfSentOnData), "AatfReports");

                case Reports.AatfReuseSitesData:
                    return RedirectToAction(nameof(AatfReuseSites), "AatfReports");

                case Reports.AatfAePublicRegister:
                    return RedirectToAction(nameof(AatfAePublicRegister), "AatfReports");

                case Reports.UkWeeeDataAtAatfs:
                    return RedirectToAction(nameof(UkWeeeDataAtAatfs), "AatfReports");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> AatfAeReturnData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfAeReturnDataViewModel();
            await PopulateFilters(model);

            return View("AatfAeReturnData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfAeReturnData(AatfAeReturnDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfAeDataCsv(int complianceYear,
            int quarter, FacilityType facilityType, ReportReturnStatus? submissionStatus, Guid? authority, Guid? pat, Guid? localArea, bool includeResubmissions)
        {
            using (var client = apiClient())
            {
                var aatfDataUrl = AatfDataUrl();

                var request = new GetAatfAeReturnDataCsv(complianceYear, quarter, facilityType, submissionStatus, authority, pat, localArea, aatfDataUrl, includeResubmissions);

                var fileData = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }

        private string AatfDataUrl()
        {
            if (HttpContext.Request.Url != null)
            {
                var url = Flurl.Url.Combine(HttpContext.Request.Url.Authority, HttpContext.Request.ApplicationPath, "/admin/aatf/details/");

                return $"{HttpContext.Request.Url.Scheme}://{url}";
            }

            return string.Empty;
        }

        private async Task PopulateFilters(AatfAeReturnDataViewModel model)
        {
            using (var client = apiClient())
            {
                model.ComplianceYears = await ComplianceYears();
                model.FacilityTypes = new SelectList(EnumHelper.GetValues(typeof(FacilityType)), "Key", "Value");
                model.CompetentAuthoritiesList = await CompetentAuthoritiesList();
                model.PanAreaList = await PatAreaList();
                model.LocalAreaList = await LocalAreaList();
            }
        }

        private async Task PopulateFilters(NonObligatedWeeeReceivedAtAatfViewModel model)
        {
            using (var client = apiClient())
            {
                model.ComplianceYears = await ComplianceYears();
            }
        }

        [HttpGet]
        public async Task<ActionResult> UkWeeeDataAtAatfs()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new UkWeeeDataAtAatfViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UkWeeeDataAtAatfs(UkWeeeDataAtAatfViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUkWeeeDataAtAatfsCsv(int complianceYear)
        {
            using (var client = apiClient())
            {
                var request = new GetUkWeeeAtAatfsCsv(complianceYear);

                var file = await client.SendAsync(User.GetAccessToken(), request);

                return File(file.Data, "text/csv", file.FileName);
            }
        }

        [HttpGet]
        public async Task<ActionResult> UkNonObligatedWeeeReceived()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new UkNonObligatedWeeeReceivedViewModel();
            await PopulateFilters(model);

            return View("UkNonObligatedWeeeReceived", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UkNonObligatedWeeeReceived(UkNonObligatedWeeeReceivedViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUkNonObligatedWeeeReceivedCsv(int complianceYear)
        {
            using (var client = apiClient())
            {
                var ukNonObligatedWeeeReceivedCsvData = await client.SendAsync(User.GetAccessToken(),
                    new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear));

                var data = new UTF8Encoding().GetBytes(ukNonObligatedWeeeReceivedCsvData.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(ukNonObligatedWeeeReceivedCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> AatfReuseSites()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfReuseSitesViewModel();
            await PopulateFilters(model);

            return View("AatfReuseSites", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfReuseSites(AatfReuseSitesViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfReuseSitesCsv(int complianceYear, Guid? authorityId, Guid? panArea)
        {
            CSVFileData fileData;

            var request = new GetAllAatfReuseSitesCsv(complianceYear, authorityId, panArea);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> AatfObligatedData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfObligatedDataViewModel();
            await PopulateFilters(model);

            return View("AatfObligatedData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfObligatedData(AatfObligatedDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfObligatedDataCsv(int complianceYear, int columnType,
            string obligationType, string aatfName, Guid? authorityId, Guid? panArea)
        {
            using (var client = apiClient())
            {
                var request = new GetAllAatfObligatedDataCsv(complianceYear, columnType, obligationType, aatfName, authorityId, panArea);
                var fileData = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> AatfNonObligatedData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new NonObligatedWeeeReceivedAtAatfViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfNonObligatedData(NonObligatedWeeeReceivedAtAatfViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfNonObligatedDataCsv(int complianceYear, string aatfName)
        {
            using (var client = apiClient())
            {
                var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(complianceYear, aatfName);

                var fileData = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> AatfSentOnData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfSentOnDataViewModel();
            await PopulateFilters(model);

            return View("AatfSentOnData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfSentOnData(AatfSentOnDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfSentOnDataCsv(int complianceYear,
            string obligationType, Guid? authorityId, Guid? panArea)
        {
            CSVFileData fileData;

            var request = new GetAllAatfSentOnDataCsv(complianceYear, obligationType, authorityId, panArea);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> AatfAePublicRegister()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new AatfAePublicRegisterViewModel();
            await PopulateFilters(model);

            return View("AatfAePublicRegister", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AatfAePublicRegister(AatfAePublicRegisterViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAatfAePublicRegisterCsv(int complianceYear,
            ReportFacilityType facilityType, Guid authorityId)
        {
            CSVFileData fileData;

            var request = new GetAatfAeDetailsCsv(complianceYear, facilityType, authorityId, null, null, true);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }
        private async Task PopulateFilters(AatfSentOnDataViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatfReturns());
            var authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name");
            }
        }
        private async Task PopulateFilters(AatfReuseSitesViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatfReturns());
            var authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name");
            }
        }

        private async Task PopulateFilters(AatfObligatedDataViewModel model)
        {
            using (var client = apiClient())
            {
                model.ComplianceYears = await ComplianceYears();
                model.CompetentAuthoritiesList = await CompetentAuthoritiesList();
                model.PanAreaList = await PatAreaList();
            }
        }

        private async Task PopulateFilters(UkWeeeDataAtAatfViewModel model)
        {
            model.ComplianceYears = await ComplianceYears();
        }

        private async Task<SelectList> ComplianceYears()
        {
            return new SelectList(await FetchComplianceYearsForAatfReturns());
        }

        private async Task PopulateFilters(UkNonObligatedWeeeReceivedViewModel model)
        {
            model.ComplianceYears = await ComplianceYears();
        }
      
        private async Task PopulateFilters(AatfAePublicRegisterViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatf());
            model.FacilityTypes = new SelectList(EnumHelper.GetValues(typeof(FacilityType)), "Key", "Value");
            model.CompetentAuthoritiesList = await CompetentAuthoritiesList();
        }
    }
}