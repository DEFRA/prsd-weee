namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Admin;
    using Core.Charges;
    using Core.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Infrastructure;
    using Services;
    using ViewModels.Charge;
    using Weee.Requests.Charges;

    public class ChargeController : AdminController
    {
        private readonly IAppConfiguration configuration;
        private readonly BreadcrumbService breadcrumb;
        private readonly Func<IWeeeClient> weeeClient;

        public ChargeController(
            IAppConfiguration configuration,
            BreadcrumbService breadcrumb,
            Func<IWeeeClient> weeeClient)
        {
            this.configuration = configuration;
            this.breadcrumb = breadcrumb;
            this.weeeClient = weeeClient;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!configuration.EnableInvoicing)
            {
                throw new InvalidOperationException("Invoicing is not enabled.");
            }

            breadcrumb.InternalActivity = "Manage PCS charges";

            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        public ActionResult SelectAuthority()
        {
            return View(new SelectAuthorityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectAuthority(SelectAuthorityViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            CompetentAuthority authority = viewModel.SelectedAuthority.Value;
            return RedirectToAction("ChooseActivity", new { authority });
        }

        [HttpGet]
        public ActionResult ChooseActivity(CompetentAuthority authority)
        {
            ViewBag.Authority = authority;
            return View(new ChooseActivityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(CompetentAuthority authority, ChooseActivityViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authority = authority;
                return View(viewModel);
            }

            switch (viewModel.SelectedActivity.Value)
            {
                case Activity.ManagePendingCharges:
                    return RedirectToAction("ManagePendingCharges", new { authority });

                case Activity.ManageIssuedCharges:
                    return RedirectToAction("IssuedCharges", new { authority });

                case Activity.ViewInvoiceRunHistory:
                    return RedirectToAction("InvoiceRuns", new { authority });

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ManagePendingCharges(CompetentAuthority authority)
        {
            IList<PendingCharge> pendingCharges;
            using (IWeeeClient client = weeeClient())
            {
                FetchPendingCharges request = new FetchPendingCharges(authority);
                pendingCharges = await client.SendAsync(User.GetAccessToken(), request);
            }

            ViewBag.Authority = authority;

            return View(pendingCharges);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManagePendingCharges(CompetentAuthority authority, FormCollection formCollection)
        {
            Guid invoiceRunId;
            using (IWeeeClient client = weeeClient())
            {
                IssuePendingCharges request = new IssuePendingCharges(authority);
                invoiceRunId = await client.SendAsync(User.GetAccessToken(), request);
            }

            return RedirectToAction("ChargesSuccessfullyIssued", new { authority, id = invoiceRunId });
        }

        [HttpGet]
        public ActionResult ChargesSuccessfullyIssued(CompetentAuthority authority, Guid id)
        {
            ViewBag.AllowDownloadOfInvoiceFiles = (authority == CompetentAuthority.England);
            return View(id);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadInvoiceFiles(CompetentAuthority authority, Guid id)
        {
            if (authority != CompetentAuthority.England)
            {
                string errorMessage = "Invoice files can only be downloaded for invoice runs related to the Environment Agency.";
                throw new InvalidOperationException(errorMessage);
            }

            FileInfo fileInfo;
            using (IWeeeClient client = weeeClient())
            {
                FetchInvoiceRunIbisZipFile request = new FetchInvoiceRunIbisZipFile(id);
                fileInfo = await client.SendAsync(User.GetAccessToken(), request);
            }

            return File(fileInfo.Data, "text/plain", fileInfo.FileName);
        }

        [HttpGet]
        public async Task<ActionResult> InvoiceRuns(CompetentAuthority authority)
        {
            IReadOnlyList<InvoiceRunInfo> invoiceRuns;
            using (IWeeeClient client = weeeClient())
            {
                FetchInvoiceRuns request = new FetchInvoiceRuns(authority);
                invoiceRuns = await client.SendAsync(User.GetAccessToken(), request);
            }

            ViewBag.AllowDownloadOfInvoiceFiles = (authority == CompetentAuthority.England);
            return View(invoiceRuns);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadChargeBreakdown(Guid id)
        {
            CSVFileData csvFileData;
            using (IWeeeClient client = weeeClient())
            {
                var request = new FetchInvoiceRunCsv(id);
                csvFileData = await client.SendAsync(User.GetAccessToken(), request);
            }

            byte[] data = new UTF8Encoding().GetBytes(csvFileData.FileContent);
            return File(data, "text/csv", CsvFilenameFormat.FormatFileName(csvFileData.FileName));
        }

        [HttpGet]
        public async Task<ActionResult> IssuedCharges(CompetentAuthority authority)
        {
            ViewBag.Authority = authority;
            ViewBag.TriggerDownload = false;

            IssuedChargesViewModel viewModel = new IssuedChargesViewModel();

            viewModel.ComplianceYears = await GetComplianceYearsWithInvoices(authority);
            viewModel.SchemeNames = await GetSchemesWithInvoices(authority);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IssuedCharges(CompetentAuthority authority, IssuedChargesViewModel viewModel)
        {
            ViewBag.Authority = authority;
            ViewBag.TriggerDownload = ModelState.IsValid;

            viewModel.ComplianceYears = await GetComplianceYearsWithInvoices(authority);
            viewModel.SchemeNames = await GetSchemesWithInvoices(authority);

            return View(viewModel);
        }

        [HttpGet]
        public Task<ActionResult> DownloadIssuedChargesCsv(CompetentAuthority authority, int complianceYear, string schemeName)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<int>> GetComplianceYearsWithInvoices(CompetentAuthority authority)
        {
            FetchComplianceYearsWithInvoices request = new FetchComplianceYearsWithInvoices(authority);
            using (IWeeeClient client = weeeClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }

        private async Task<IEnumerable<string>> GetSchemesWithInvoices(CompetentAuthority authority)
        {
            FetchSchemesWithInvoices request = new FetchSchemesWithInvoices(authority);
            using (IWeeeClient client = weeeClient())
            {
                return await client.SendAsync(User.GetAccessToken(), request);
            }
        }
    }
}