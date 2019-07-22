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
    using Core.Admin;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using Infrastructure;
    using Prsd.Core;
    using Prsd.Core.Helpers;
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
                    return RedirectToAction(nameof(ProducerDetails));

                case Reports.ProducerPublicRegister:
                    return RedirectToAction(nameof(ProducerPublicRegister));

                case Reports.UkWeeeData:
                    return RedirectToAction(nameof(UkWeeeData));

                case Reports.UkWeeeDataAtAatfs:
                    return RedirectToAction(nameof(UkWeeeDataAtAatfs));

                case Reports.ProducerEeeData:
                    return RedirectToAction(nameof(ProducerEeeData));

                case Reports.SchemeWeeeData:
                    return RedirectToAction(nameof(SchemeWeeeData));

                case Reports.UkEeeData:
                    return RedirectToAction(nameof(UkEeeData));

                case Reports.SchemeObligationData:
                    return RedirectToAction(nameof(SchemeObligationData));

                case Reports.MissingProducerData:
                    return RedirectToAction(nameof(MissingProducerData));

                case Reports.AatfAeReturnData:
                    return RedirectToAction("AatfAeReturnData");

                case Reports.AatfObligatedData:
                    return RedirectToAction(nameof(AatfObligatedData));

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
        public async Task<ActionResult> DownloadProducerDetailsCsv(int complianceYear, Guid? schemeId, Guid? authorityId,
            bool includeRemovedProducers, bool includeBrandNames)
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
                GetMemberDetailsCsv request = 
                    new GetMemberDetailsCsv(complianceYear, includeRemovedProducers, schemeId, authorityId, includeBrandNames);
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

            GetProducerEeeDataCsv request = new GetProducerEeeDataCsv(complianceYear, schemeId, obligationType);
            using (IWeeeClient client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> SchemeWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            ProducersDataViewModel model = new ProducersDataViewModel();
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

            GetSchemeWeeeCsv request = new GetSchemeWeeeCsv(complianceYear, schemeId, obligationType);
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

            GetUkWeeeCsv request = new GetUkWeeeCsv(complianceYear);
            using (var client = apiClient())
            {
                file = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(file.Data, "text/csv", file.FileName);
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
                // TODO: Implement GetUkWeeeAtAatfsCsv handler
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

        [HttpGet]
        public ActionResult SchemeObligationData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            SchemeObligationDataViewModel model = new SchemeObligationDataViewModel();
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
                byte[] data = new UTF8Encoding().GetBytes(schemeObligationCsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(schemeObligationCsvData.FileName));
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> MissingProducerData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            MissingProducerDataViewModel model = new MissingProducerDataViewModel();
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

            GetMissingProducerDataCsv request = new GetMissingProducerDataCsv(complianceYear, obligationType, quarter, schemeId);
            using (IWeeeClient client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> AatfAeReturnData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            AatfAeReturnDataViewModel model = new AatfAeReturnDataViewModel();
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
          int quarter,  FacilityType facilityType, int? submissionStatus, Guid? authority, Guid? pat, Guid? localArea)
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

        [HttpGet]
        public async Task<ActionResult> AatfObligatedData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            AatfObligatedDataViewModel model = new AatfObligatedDataViewModel();
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

            GetAllAatfObligatedDataCsv request = new GetAllAatfObligatedDataCsv(complianceYear, columnType, obligationType, aatfName, authorityId, panArea);
            using (IWeeeClient client = apiClient())
            {
                fileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(fileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
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

        private async Task PopulateFilters(UkWeeeDataAtAatfViewModel model)
        {
            var years = await FetchComplianceYearsForAatfReturns();
            model.ComplianceYears = new SelectList(years);
        }

        private async Task PopulateFilters(UkEeeDataViewModel model)
        {
            List<int> years = await FetchComplianceYearsForDataReturns();
            model.ComplianceYears = new SelectList(years);
        }

        private void PopulateFilters(SchemeObligationDataViewModel model)
        {
            model.ComplianceYears = new SelectList(FetchAllComplianceYears());
        }

        private async Task PopulateFilters(ProducersDataViewModel model, bool populateSchemes)
        {
            List<int> years = await FetchComplianceYearsForDataReturns();
            model.ComplianceYears = new SelectList(years);

            if (populateSchemes)
            {
                List<SchemeData> schemes = await FetchSchemes();
                model.Schemes = new SelectList(schemes, "Id", "SchemeName");
            }
        }

        private async Task PopulateFilters(MissingProducerDataViewModel model)
        {
            List<int> years = await FetchComplianceYearsForDataReturns();
            model.ComplianceYears = new SelectList(years);

            List<SchemeData> schemes = await FetchSchemes();
            model.Schemes = new SelectList(schemes, "Id", "SchemeName");
        }

        private async Task PopulateFilters(AatfAeReturnDataViewModel model)
        {
            model.ComplianceYears = new SelectList(FetchAllAATFComplianceYears());
            model.FacilityTypes = new SelectList(EnumHelper.GetValues(typeof(FacilityType)), "Key", "Value");
            IList<UKCompetentAuthorityData> authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name"); 
                model.LocalAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetLocalAreas()), "Id", "Name");
            }
        }

        private async Task PopulateFilters(AatfObligatedDataViewModel model)
        {
            model.ComplianceYears = new SelectList(FetchAllAATFComplianceYears());
            IList<UKCompetentAuthorityData> authorities = await FetchAuthorities();
            model.CompetentAuthoritiesList = new SelectList(authorities, "Id", "Abbreviation");
            using (var client = apiClient())
            {
                model.PanAreaList = new SelectList(await client.SendAsync(User.GetAccessToken(), new GetPanAreas()), "Id", "Name");
            }
        }

        /// <summary>
        /// Return all years from 2019 to the current year in descending order
        /// </summary>
        /// <returns>descending list of aatf/ae compliance years</returns>
        private List<int> FetchAllAATFComplianceYears()
        {
            return Enumerable.Range(2019, DateTime.Now.Year - 2018)
                .OrderByDescending(year => year)
                .ToList();
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

        private async Task<List<int>> FetchComplianceYearsForAatfReturns()
        {
            var request = new GetAatfReturnsActiveComplianceYears();
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