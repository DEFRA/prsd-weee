namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Services;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.EvidenceReports;

    public class EvidenceReportsController : ReportsBaseController
    {
        private readonly ConfigurationService configurationService;

        public EvidenceReportsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, ConfigurationService configurationService) : 
            base(apiClient, breadcrumb)
        {
            this.configurationService = configurationService;
        }

        [HttpGet]
        public async Task<ActionResult> EvidenceNoteReport()
        {
            SetBreadcrumb();

            var model = new EvidenceReportViewModel();

            return View(model);
        }
    }
}