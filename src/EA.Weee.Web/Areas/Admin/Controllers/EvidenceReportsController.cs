namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Infrastructure;
    using Prsd.Core.Helpers;
    using ViewModels.EvidenceReports;
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
        public async Task<ActionResult> EvidenceNoteReport()
        {
            SetBreadcrumb();

            var model = new EvidenceReportViewModel();

            var returnsDate = configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom;

            using (var client = ApiClient())
            {
                var currentDate = await client.SendAsync(User.GetAccessToken(), new GetApiUtcDate());

                var complianceYears = Enumerable.Range(returnsDate.Year, currentDate.Year - returnsDate.Year)
                    .OrderByDescending(x => x)
                    .ToList();

                model.ComplianceYears = new SelectList(complianceYears);
                model.TonnageToDisplayOptions = new SelectList(EnumHelper.GetValues(typeof(TonnageToDisplayReportEnum)), "Key", "Value");

                model.SelectedYear = complianceYears.ElementAt(0);
                model.SelectedTonnageToDisplay = TonnageToDisplayReportEnum.OriginalTonnages;
            }

            return View(model);
        }
    }
}