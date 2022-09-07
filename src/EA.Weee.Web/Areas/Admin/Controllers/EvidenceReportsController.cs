namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Services;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using ViewModels.EvidenceReports;
    using ViewModels.Reports;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Requests.Shared;

    public class EvidenceReportsController : ReportsBaseController
    {
        private readonly ConfigurationService configurationService;

        public EvidenceReportsController(Func<IWeeeClient> apiClient, 
            BreadcrumbService breadcrumb, 
            ConfigurationService configurationService) : 
            base(apiClient, breadcrumb)
        {
            this.configurationService = configurationService;
        }

        [HttpGet]
        public async Task<ActionResult> ChooseReport()
        {
            SetBreadcrumb();

            async Task<ActionResult> ViewAction()
            {
                return await Task.Run(() =>
                {
                    var model = new ChooseEvidenceReportViewModel();

                    return View(model);
                });
            }

            return await CheckUserStatus("EvidenceReports", "ChooseReport", ViewAction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReport(ChooseEvidenceReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.SelectedValue)
            {
                case Reports.EvidenceNoteData:
                    return RedirectToAction(nameof(EvidenceNoteReport), "EvidenceReports");

                case Reports.EvidenceTransfersData:
                    return RedirectToAction("Index", "AdminHolding");

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> EvidenceNoteReport()
        {
            SetBreadcrumb();

            ViewBag.TriggerDownload = false;

            async Task<ActionResult> ViewAction()
            {
                var model = new EvidenceReportViewModel();

                await SetupEvidenceReportViewModelFilters(model);

                return View(model);
            }

            return await CheckUserStatus("EvidenceReports", "ChooseReport", ViewAction);
        }

        public async Task<EvidenceReportViewModel> SetupEvidenceReportViewModelFilters(EvidenceReportViewModel model)
        {
            using (var client = ApiClient())
            {
                var returnsDate = configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom;
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());
                var complianceYears = Enumerable.Range(returnsDate.Year, (currentDate.Year - returnsDate.Year) + 1).OrderByDescending(x => x).ToList();

                model.TonnageToDisplayOptions = new SelectList(EnumHelper.GetValues(typeof(TonnageToDisplayReportEnum)),
                    "Key", "Value");
                model.ComplianceYears = new SelectList(complianceYears);

                return model;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EvidenceNoteReport(EvidenceReportViewModel model)
        {
            ViewBag.TriggerDownload = ModelState.IsValid;

            await SetupEvidenceReportViewModelFilters(model);
            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadEvidenceNoteReport(int complianceYear, TonnageToDisplayReportEnum tonnageToDisplay)
        {
            async Task<ActionResult> FileResult()
            {
                using (var client = ApiClient())
                {
                    var request = new GetEvidenceNoteReportRequest(null, null, tonnageToDisplay, complianceYear);

                    var file = await client.SendAsync(User.GetAccessToken(), request);

                    var data = new UTF8Encoding().GetBytes(file.FileContent);
                    return File(data, "text/csv", CsvFilenameFormat.FormatFileName(file.FileName));
                }
            }

            return await CheckUserStatus("EvidenceReports", "ChooseReport", FileResult);
        }
    }
}