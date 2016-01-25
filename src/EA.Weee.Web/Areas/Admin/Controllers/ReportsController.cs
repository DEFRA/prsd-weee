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

                case Reports.UKWeeeData:
                    return RedirectToAction("UKWeeeData");

                case Reports.ProducerEeeData:
                    return RedirectToAction("ProducerEeeData");

                case Reports.SchemeWeeeData:
                    return RedirectToAction("SchemeWeeeData");

                case Reports.UKEEEData:
                    return RedirectToAction("UKEEEData");

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

            await SetReportsFilterLists(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerDetails(ReportsFilterViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await SetReportsFilterLists(model);

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
                GetMemberDetailsCSV request = new GetMemberDetailsCSV(complianceYear, includeRemovedProducers, schemeId, authorityId);
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

            await SetReportsFilterLists(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerPublicRegister(ProducerPublicRegisterViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            await SetReportsFilterLists(model);

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

            List<int> years = await FetchComplianceYearsForDataReturns();

            ProducersDataViewModel model = new ProducersDataViewModel();
            model.ComplianceYears = new SelectList(years);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProducerEeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            List<int> years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadProducerEeeDataCSV(int complianceYear, ObligationType obligationType)
        {
            using (IWeeeClient client = apiClient())
            {
                var producerEEECsvData = await client.SendAsync(User.GetAccessToken(),
                   new GetProducerEEEDataCSV(complianceYear, obligationType));

                byte[] data = new UTF8Encoding().GetBytes(producerEEECsvData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerEEECsvData.FileName));
            }
        }

        [HttpGet]
        public async Task<ActionResult> SchemeWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            List<int> years = await FetchComplianceYearsForDataReturns();

            ProducersDataViewModel model = new ProducersDataViewModel();
            model.ComplianceYears = new SelectList(years);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SchemeWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            List<int> years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);

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
        public async Task<ActionResult> UKWeeeData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            List<int> years = await FetchComplianceYearsForDataReturns();

            ProducersDataViewModel model = new ProducersDataViewModel();

            model.ComplianceYears = new SelectList(years);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UKWeeeData(ProducersDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;

            List<int> years = await FetchComplianceYearsForDataReturns();

            model.ComplianceYears = new SelectList(years);

            ViewBag.TriggerDownload = ModelState.IsValid;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadUKWeeeDataCsv(int complianceYear)
        {
            FileInfo file;

            GetUKWeeeCsv request = new GetUKWeeeCsv(complianceYear);
            using (var client = apiClient())
            {
                file = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(file.Data, "text/csv", file.FileName);
        }

        [HttpGet]
        public async Task<ActionResult> UKEEEData()
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = false;

            List<int> years;
            try
            {
                years = await FetchComplianceYearsForDataReturns();
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);
                if (ModelState.IsValid)
                {
                    throw;
                }
                return View();
            }

            UKEEEDataViewModel model = new UKEEEDataViewModel();
            model.ComplianceYears = new SelectList(years);
            return View("UKEEEData", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UKEEEData(UKEEEDataViewModel model)
        {
            SetBreadcrumb();
            ViewBag.TriggerDownload = ModelState.IsValid;
            List<int> years;
            try
            {
                years = await FetchComplianceYearsForDataReturns();
            }
            catch (ApiBadRequestException ex)
            {
                this.HandleBadRequest(ex);
                if (ModelState.IsValid)
                {
                    throw;
                }
                return View();
            }

            model.ComplianceYears = new SelectList(years);
            ViewBag.TriggerDownload = ModelState.IsValid;
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

        private async Task<List<int>> FetchComplianceYearsForDataReturns()
        {
            var request = new GetDataReturnsActiveComplianceYears();
            using (var client = apiClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private async Task SetReportsFilterLists(ReportsFilterViewModel model)
        {
            using (IWeeeClient client = apiClient())
            {
                var allYears = await client.SendAsync(User.GetAccessToken(), new GetMemberRegistrationsActiveComplianceYears());

                var appropriateAuthorities = await client.SendAsync(User.GetAccessToken(), new GetUKCompetentAuthorities());

                model.ComplianceYears = new SelectList(allYears);

                model.AppropriateAuthorities = new SelectList(appropriateAuthorities, "Id", "Abbreviation");

                if (model.FilterByScheme)
                {
                    var allSchemes = await client.SendAsync(User.GetAccessToken(), new GetAllApprovedSchemes());
                    model.SchemeNames = new SelectList(allSchemes, "Id", "SchemeName");
                }
            }
        }

        private async Task SetReportsFilterLists(ProducerPublicRegisterViewModel model)
        {
            using (IWeeeClient client = apiClient())
            {
                var allYears = await client.SendAsync(User.GetAccessToken(), new GetMemberRegistrationsActiveComplianceYears());
                model.ComplianceYears = new SelectList(allYears);
            }
        }

        private void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ViewReports;
        }

        private async Task<ActionResult> DownloadMembersDetailsCSV(ReportsFilterViewModel model, IWeeeClient client)
        {
            string approvalnumber = string.Empty;
            string csvFileName = string.Format("{0}_producerdetails_{1}.csv", model.SelectedYear, DateTime.Now.ToString("ddMMyyyy_HHmm"));
            if (model.SelectedScheme.HasValue)
            {
                SchemeData scheme =
                    await client.SendAsync(User.GetAccessToken(), new GetSchemeById(model.SelectedScheme.Value));
                approvalnumber = scheme.ApprovalName;
                csvFileName = string.Format("{0}_{1}_producerdetails_{2}.csv", model.SelectedYear,
                approvalnumber, DateTime.Now.ToString("ddMMyyyy_HHmm"));
            }
            if (model.SelectedAA.HasValue)
            {
                UKCompetentAuthorityData authorityData =
                    await
                        client.SendAsync(User.GetAccessToken(),
                            new GetUKCompetentAuthorityById(model.SelectedAA.Value));
                var authorisedAuthorityName = authorityData.Abbreviation;
                csvFileName = string.Format("{0}_{1}_{2}_producerdetails_{3}.csv", model.SelectedYear,
               approvalnumber, authorisedAuthorityName, DateTime.Now.ToString("ddMMyyyy_HHmm"));
            }

            var membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(),
                new GetMemberDetailsCSV(model.SelectedYear, model.IncludeRemovedProducer, model.SelectedScheme, model.SelectedAA));

            byte[] data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(csvFileName));
        }

        private async Task<ActionResult> DownloadProducerPublicRegisterCSV(ProducerPublicRegisterViewModel model, IWeeeClient client)
        {
            var membersDetailsCsvData = await client.SendAsync(User.GetAccessToken(),
               new GetProducerPublicRegisterCSV(model.SelectedYear));

            byte[] data = new UTF8Encoding().GetBytes(membersDetailsCsvData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(membersDetailsCsvData.FileName));
        }

        private async Task<ActionResult> DownloadProducerEEEDataCSV(ProducersDataViewModel model, IWeeeClient client)
        {
            var producerEEECsvData = await client.SendAsync(User.GetAccessToken(),
               new GetProducerEEEDataCSV(model.SelectedYear, model.SelectedObligationType));

            byte[] data = new UTF8Encoding().GetBytes(producerEEECsvData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(producerEEECsvData.FileName));
        }
    }
}