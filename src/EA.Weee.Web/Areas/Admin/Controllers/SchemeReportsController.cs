namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Core.Admin;
    using Core.Shared;
    using Infrastructure;
    using Prsd.Core;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Extensions;
    using ViewModels.Reports;
    using ViewModels.SchemeReports;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Requests.Admin;
    using Weee.Requests.Admin.AatfReports;
    using Weee.Requests.Admin.GetActiveComplianceYears;
    using Weee.Requests.Admin.Reports;
    using Weee.Requests.Scheme;
    using Weee.Requests.Shared;

    public class SchemeReportsController : ReportsBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly ConfigurationService configurationService;

        public SchemeReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, ConfigurationService configurationService) : base(apiClient, breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.configurationService = configurationService;
        }

        public async Task<ActionResult> Index()
        {
            SetBreadcrumb();

            using (var client = apiClient())
            {
                var userStatus = await client.SendAsync(User.GetAccessToken(), new GetAdminUserStatus(User.GetUserId()));

                switch (userStatus)
                {
                    case UserStatus.Active:
                        return RedirectToAction("ChooseReport", "SchemeReports");
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

            var model = new ChooseSchemeReportViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseSchemeReportViewModel model)
        {
            SetBreadcrumb();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.ProducerDetails:
                    return RedirectToAction(nameof(ProducerDetails), "SchemeReports");

                case Reports.ProducerPublicRegister:
                    return RedirectToAction(nameof(ProducerPublicRegister), "SchemeReports");

                case Reports.UkWeeeData:
                    return RedirectToAction(nameof(UkWeeeData), "SchemeReports");

                case Reports.ProducerEeeData:
                    return RedirectToAction(nameof(ProducerEeeData), "SchemeReports");

                case Reports.SchemeWeeeData:
                    return RedirectToAction(nameof(SchemeWeeeData), "SchemeReports");

                case Reports.UkEeeData:
                    return RedirectToAction(nameof(UkEeeData), "SchemeReports");

                case Reports.SchemeObligationData:
                    return RedirectToAction(nameof(SchemeObligationData), "SchemeReports");

                case Reports.MissingProducerData:
                    return RedirectToAction(nameof(MissingProducerData), "SchemeReports");

                case Reports.PcsEvidenceAndObligationProgressData:
                    return RedirectToAction(nameof(EvidenceAndObligationProgress), "SchemeReports");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> EvidenceAndObligationProgress(int? selectedYear)
        {
            SetBreadcrumb();

            ViewBag.TriggerDownload = false;

            var model = new EvidenceAndObligationProgressViewModel
            {
                SelectedYear = selectedYear ?? 0
            };

            await PopulateFilters(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EvidenceAndObligationProgress(EvidenceAndObligationProgressViewModel model)
        {
            SetBreadcrumb();

            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceAndObligationProgressCsv(int complianceYear, Guid? schemeId)
        {
            using (var client = apiClient())
            {
                var request =
                    new GetSchemeObligationAndEvidenceTotalsReportRequest(schemeId, null, null, complianceYear);

                var csvReport = await client.SendAsync(User.GetAccessToken(), request);

                var data = new UTF8Encoding().GetBytes(csvReport.FileContent);

                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(csvReport.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProducerDetails()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new ReportsFilterViewModel();
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
        public async Task<ActionResult> DownloadProducerDetailsCsv(int complianceYear, Guid? schemeId, Guid? authorityId,
            bool includeRemovedProducers, bool includeBrandNames)
        {
            var fileName = new StringBuilder();

            fileName.AppendFormat("{0:D4}", complianceYear);

            if (schemeId != null)
            {
                using (var client = apiClient())
                {
                    var requestScheme = new GetSchemeById(schemeId.Value);
                    var scheme = await client.SendAsync(User.GetAccessToken(), requestScheme);

                    fileName.AppendFormat("_{0}", scheme.ApprovalName);
                }
            }

            if (authorityId != null)
            {
                using (var client = apiClient())
                {
                    var requestAuthority = new GetUKCompetentAuthorityById(authorityId.Value);
                    var authorityData = await client.SendAsync(User.GetAccessToken(), requestAuthority);

                    fileName.AppendFormat("_{0}", authorityData.Abbreviation);
                }
            }

            fileName.AppendFormat("_producerdetails_{0:ddMMyyyy_HHmm}.csv", SystemTime.UtcNow);

            CSVFileData membersDetailsCsvData;
            using (var client = apiClient())
            {
                var request =
                    new GetMemberDetailsCsv(complianceYear, includeRemovedProducers, schemeId, authorityId, includeBrandNames);
                membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);

            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileName.ToString()));
        }

        [HttpGet]
        public async Task<ActionResult> ProducerPublicRegister()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new ProducerPublicRegisterViewModel();
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
            using (var client = apiClient())
            {
                var membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(),
                   new GetProducerPublicRegisterCSV(complianceYear));

                var data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(membersDetailsCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProducerEeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new ProducersDataViewModel();
            await PopulateFilters(model, true);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerEeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model, true);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerEeeDataCsv(int complianceYear, Guid? schemeId, ObligationType obligationType)
        {
            CSVFileData fileData;

            var request = new GetProducerEeeDataCsv(complianceYear, schemeId, obligationType);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> SchemeWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new ProducersDataViewModel();
            await PopulateFilters(model, true);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SchemeWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model, true);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadSchemeWeeeDataCsv(int complianceYear, Guid? schemeId, ObligationType obligationType)
        {
            FileInfo file;

            var request = new GetSchemeWeeeCsv(complianceYear, schemeId, obligationType);
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

            var model = new ProducersDataViewModel();
            await PopulateFilters(model, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UkWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model, false);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUkWeeeDataCsv(int complianceYear)
        {
            FileInfo file;

            var request = new GetUkWeeeCsv(complianceYear);
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

            var model = new UkEeeDataViewModel();
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
                var data = new UTF8Encoding().GetBytes(ukEeeCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(ukEeeCsvData.FileName));
            }
        }

        [HttpGet]
        public ActionResult SchemeObligationData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new SchemeObligationDataViewModel();
            PopulateFilters(model);

            return View("SchemeObligationData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SchemeObligationData(SchemeObligationDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadSchemeObligationDataCsv(int complianceYear)
        {
            using (var client = apiClient())
            {
                var schemeObligationCsvData = await client.SendAsync(User.GetAccessToken(),
                    new GetSchemeObligationDataCsv(complianceYear));
                var data = new UTF8Encoding().GetBytes(schemeObligationCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(schemeObligationCsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> MissingProducerData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            var model = new MissingProducerDataViewModel();
            await PopulateFilters(model);

            return View("MissingProducerData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MissingProducerData(MissingProducerDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await PopulateFilters(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadMissingProducerDataCsv(int complianceYear,
            Guid? schemeId, ObligationType obligationType, int? quarter)
        {
            CSVFileData fileData;

            var request = new GetMissingProducerDataCsv(complianceYear, obligationType, quarter, schemeId);
            using (var client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            var data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        private async Task PopulateFilters(EvidenceAndObligationProgressViewModel model)
        {
            var years = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(
                configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom,
                await FetchCurrentSystemDate());

            var schemes = await FetchSchemesWithObligationOrEvidence(model.SelectedYear);

            model.ComplianceYears = new SelectList(years);
            model.Schemes = new SelectList(schemes, "Id", "SchemeName");
        }

        private async Task PopulateFilters(ReportsFilterViewModel model)
        {
            var years = await FetchComplianceYearsForMemberRegistrations();
            var schemes = await FetchSchemes();
            var authorities = await FetchAuthorities();

            model.ComplianceYears = new SelectList(years);
            model.SchemeNames = new SelectList(schemes, "Id", "SchemeName");
            model.AppropriateAuthorities = new SelectList(authorities, "Id", "Abbreviation");
        }

        private async Task PopulateFilters(ProducerPublicRegisterViewModel model)
        {
            var years = await FetchComplianceYearsForMemberRegistrations();

            model.ComplianceYears = new SelectList(years);
        }

        private void PopulateFilters(SchemeObligationDataViewModel model)
        {
            model.ComplianceYears = new SelectList(FetchAllComplianceYears());
        }

        private async Task PopulateFilters(ProducersDataViewModel model, bool populateSchemes)
        {
            var years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);

            if (populateSchemes)
            {
                var schemes = await FetchSchemes();
                model.Schemes = new SelectList(schemes, "Id", "SchemeName");
            }
        }

        private async Task PopulateFilters(MissingProducerDataViewModel model)
        {
            var years = await FetchComplianceYearsForDataReturns();
            model.ComplianceYears = new SelectList(years);

            var schemes = await FetchSchemes();
            model.Schemes = new SelectList(schemes, "Id", "SchemeName");
        }

        /// <summary>
        /// Return all years from 2016 to the current year in descending order
        /// </summary>
        /// <returns>descending list of compliance years</returns>
        private List<int> FetchAllComplianceYears()
        {
            return Enumerable.Range(2016, DateTime.Now.Year - 2015)
                .OrderByDescending(year => year)
                .ToList();
        }

        /// <summary>
        /// Return all years with valid data returns uploads associated with them
        /// </summary>
        /// <returns></returns>
        private async Task<List<int>> FetchComplianceYearsForDataReturns()
        {
            var request = new GetDataReturnsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }
        private async Task PopulateFilters(UkEeeDataViewModel model)
        {
            var years = await FetchComplianceYearsForDataReturns();
            model.ComplianceYears = new SelectList(years);
        }

        private async Task<List<int>> FetchComplianceYearsForMemberRegistrations()
        {
            var request = new GetMemberRegistrationsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private async Task<DateTime> FetchCurrentSystemDate()
        {
            using (var client = ApiClient())
            {
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                return currentDate;
            }
        }
    }
}