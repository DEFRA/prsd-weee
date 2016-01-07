namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
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

            breadcrumb.InternalActivity = "Manage charges";

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
                    throw new NotImplementedException();

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
                string errorMessage = "Invoice files can only be downloaded for invoice runs related to the Environnent Agency.";
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

        public Task<ActionResult> InvoiceRuns(CompetentAuthority authority)
        {
            throw new NotImplementedException();
        }
    }
}