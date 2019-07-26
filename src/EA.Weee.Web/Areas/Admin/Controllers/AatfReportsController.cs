namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.AatfReturn;
    using Core.Admin;
    using Core.Shared;
    using EA.Weee.Requests.Admin.AatfReports;
    using EA.Weee.Web.Areas.Admin.ViewModels.AatfReports;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using Services;
    using ViewModels.Home;
    using ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.Aatf;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Shared;

    public class AatfReportsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public AatfReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
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
            int quarter, FacilityType facilityType, int? submissionStatus, Guid? authority, Guid? pat, Guid? localArea)
        {
            CSVFileData fileData;
            var aatfDataUrl = AatfDataUrl();

            var request = new GetAatfAeReturnDataCsv(complianceYear, quarter, facilityType, submissionStatus, authority, pat, localArea, aatfDataUrl);

            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
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
            model.ComplianceYears = new SelectList(FetchAllAATFComplianceYears());
            model.FacilityTypes = new SelectList(EnumHelper.GetValues(typeof(FacilityType)), "Key", "Value");
            var authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name");
                model.LocalAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetLocalAreas()), "Id", "Name");
            }
        }

        private async Task<IList<UKCompetentAuthorityData>> FetchAuthorities()
        {
            var request = new GetUKCompetentAuthorities();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
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
            FileInfo file;

            var request = new GetUkWeeeAtAatfsCsv(complianceYear);
            using (var client = apiClient())
            {
                file = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(file.Data, "text/csv", file.FileName);
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
            CSVFileData fileData;

            var request = new GetAllAatfObligatedDataCsv(complianceYear, columnType, obligationType, aatfName, authorityId, panArea);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
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
            string obligationType, string aatfName, Guid? authorityId, Guid? panArea)
        {
            CSVFileData fileData;

            var request = new GetAllAatfSentOnDataCsv(complianceYear, obligationType, aatfName, authorityId, panArea);
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

        private async Task PopulateFilters(AatfObligatedDataViewModel model)
        {
            model.ComplianceYears = new SelectList(await FetchComplianceYearsForAatfReturns());
            var authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name");
            }
        }

        private async Task PopulateFilters(UkWeeeDataAtAatfViewModel model)
        {
            var years = await FetchComplianceYearsForAatfReturns();
            model.ComplianceYears = new SelectList(years);
        }

        private async Task PopulateFilters(UkNonObligatedWeeeReceivedViewModel model)
        {
            var years = await FetchComplianceYearsForAatfReturns();
            model.ComplianceYears = new SelectList(years);
        }

        private async Task<List<int>> FetchComplianceYearsForAatfReturns()
        {
            var request = new GetAatfReturnsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private IEnumerable<int> FetchAllAATFComplianceYears()
        {
            return Enumerable.Range(2019, DateTime.Now.Year - 2018)
                .OrderByDescending(year => year)
                .ToList();
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ViewReports;
        }
    }
}