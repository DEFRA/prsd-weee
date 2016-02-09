namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Base;
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core;
    using Prsd.Core.Web.ApiClient;
    using Prsd.Core.Web.Mvc.Extensions;
    using Services;
    using ViewModels.Home;
    using ViewModels.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;
    using GetSchemes = Weee.Requests.Admin.GetSchemes;

    public class ReportsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public ReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb)
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
                            string.Format("Cannot determine result for user with status '{0}'", userStatus));
                }
            }
        }

        [HttpGet]
        public ActionResult ChooseReport()
        {
            SetBreadcrumb();

            var model = new ChooseReportViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseReportViewModel model)
        {
            SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.ProducerDetails:
                    return RedirectToAction("ProducerDetails");

                case Reports.ProducerPublicRegister:
                    return RedirectToAction("ProducerPublicRegister");

                case Reports.UkWeeeData:
                    return RedirectToAction("UkWeeeData");

                case Reports.ProducerEeeData:
                    return RedirectToAction("ProducerEeeData");

                case Reports.SchemeWeeeData:
                    return RedirectToAction("SchemeWeeeData");

                case Reports.UkEeeData:
                    return RedirectToAction("UkEeeData");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProducerDetails()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ReportsFilterViewModel model = new ReportsFilterViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerDetails(ReportsFilterViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerDetailsCsv(int complianceYear, Guid? schemeId, Guid? authorityId, bool includeRemovedProducers)
        {
            StringBuilder fileName = new StringBuilder();

            fileName.AppendFormat("{0:D4}", complianceYear);

            if (schemeId != null)
            {
                using (IWeeeClient client = apiClient())
                {
                    GetSchemeById requestScheme = new GetSchemeById(schemeId.Value);
                    SchemeData scheme = await client.SendAsync(User.GetAccessToken(), requestScheme);

                    fileName.AppendFormat("_{0}", scheme.ApprovalName);
                }
            }

            if (authorityId != null)
            {
                using (IWeeeClient client = apiClient())
                {
                    GetUKCompetentAuthorityById requestAuthority = new GetUKCompetentAuthorityById(authorityId.Value);
                    UKCompetentAuthorityData authorityData = await client.SendAsync(User.GetAccessToken(), requestAuthority);

                    fileName.AppendFormat("_{0}", authorityData.Abbreviation);
                }
            }

            fileName.AppendFormat("_producerdetails_{0:ddMMyyyy_HHmm}.csv", SystemTime.UtcNow);

            CSVFileData membersDetailsCsvData;
            using (IWeeeClient client = apiClient())
            {
                GetMemberDetailsCsv request = new GetMemberDetailsCsv(complianceYear, includeRemovedProducers, schemeId, authorityId);
                membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);

            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileName.ToString()));
        }

        [HttpGet]
        public async Task<ActionResult> ProducerPublicRegister()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ProducerPublicRegisterViewModel model = new ProducerPublicRegisterViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerPublicRegister(ProducerPublicRegisterViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerPublicRegisterCsv(int complianceYear)
        {
            using (IWeeeClient client = apiClient())
            {
                var membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(),
                   new GetProducerPublicRegisterCSV(complianceYear));

                byte[] data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(membersDetailsCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProducerEeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ProducersDataViewModel model = new ProducersDataViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerEeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerEeeDataCsv(int complianceYear, ObligationType obligationType)
        {
            using (IWeeeClient client = apiClient())
            {
                var producerEeeCsvData = await client.SendAsync(User.GetAccessToken(),
                   new GetProducerEeeDataCsv(complianceYear, obligationType));

                byte[] data = new UTF8Encoding().GetBytes(producerEeeCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerEeeCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> SchemeWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ProducersDataViewModel model = new ProducersDataViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SchemeWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadSchemeWeeeDataCsv(int complianceYear, ObligationType obligationType)
        {
            FileInfo file;

            GetSchemeWeeeCsv request = new GetSchemeWeeeCsv(complianceYear, obligationType);
            using (var client = apiClient())
            {
                file = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(file.Data, "text/csv", file.FileName);
        }

        [HttpGet]
        public async Task<ActionResult> UkWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ProducersDataViewModel model = new ProducersDataViewModel();
            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UkWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUkWeeeDataCsv(int complianceYear)
        {
            FileInfo file;

            GetUkWeeeCsv request = new GetUkWeeeCsv(complianceYear);
            using (var client = apiClient())
            {
                file = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(file.Data, "text/csv", file.FileName);
        }

        [HttpGet]
        public async Task<ActionResult> UkEeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            UkEeeDataViewModel model = new UkEeeDataViewModel();
            await PopulateFilters(model);

            return View("UkEeeData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UkEeeData(UkEeeDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUkEeeDataCsv(int complianceYear)
        {   
            using (var client = apiClient())
            {
                var ukEeeCsvData = await client.SendAsync(User.GetAccessToken(),
                    new GetUkEeeDataCsv(complianceYear));
                byte[] data = new UTF8Encoding().GetBytes(ukEeeCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(ukEeeCsvData.FileName));
            }
        }

        private async Task PopulateFilters(ReportsFilterViewModel model)
        {
            List<int> years = await FetchComplianceYearsForMemberRegistrations();
            List<SchemeData> schemes = await FetchSchemes();
            IList<UKCompetentAuthorityData> authorities = await FetchAuthorities();

            model.ComplianceYears = new SelectList(years);
            model.SchemeNames = new SelectList(schemes, "Id", "SchemeName");
            model.AppropriateAuthorities = new SelectList(authorities, "Id", "Abbreviation");
        }

        private async Task PopulateFilters(ProducerPublicRegisterViewModel model)
        {
            List<int> years = await FetchComplianceYearsForMemberRegistrations();

            model.ComplianceYears = new SelectList(years);
        }

        private async Task PopulateFilters(UkEeeDataViewModel model)
        {
            List<int> years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);
        }

        private async Task PopulateFilters(ProducersDataViewModel model)
        {
            List<int> years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);
        }

        private async Task<List<int>> FetchComplianceYearsForDataReturns()
        {
            var request = new GetDataReturnsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private async Task<List<int>> FetchComplianceYearsForMemberRegistrations()
        {
            var request = new GetMemberRegistrationsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private async Task<List<SchemeData>> FetchSchemes()
        {
            var request = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
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

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ViewReports;
        }
    }
}