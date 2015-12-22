namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Charges;
    using Core.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;
    using ViewModels.Charge;

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
            return View(new ChooseActivityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(CompetentAuthority authority, ChooseActivityViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            switch (viewModel.SelectedActivity.Value)
            {
                case Activity.ManagePendingCharges:
                    return RedirectToAction("ManagePendingCharges", new { authority });

                case Activity.ManageIssuedCharges:
                    throw new NotImplementedException();

                case Activity.ViewInvoiceRunHistory:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException();
            }
        }

        [HttpGet]
        public ActionResult ManagePendingCharges(CompetentAuthority authority)
        {
            IReadOnlyCollection<PendingCharge> pendingCharges;
            using (IWeeeClient client = weeeClient())
            {
                // TODO: data access
                pendingCharges = new List<PendingCharge>(TemporaryFakeDataAccess());
            }

            ViewBag.Authority = authority;

            return View(pendingCharges);
        }

        private IEnumerable<PendingCharge> TemporaryFakeDataAccess()
        {
            yield return new PendingCharge()
            {
                SchemeName = "Another Cool Scheme",
                SchemeApprovalNumber = "WEEE/12345/01",
                ComplianceYear = 2017,
                TotalGBP = 32089.40m
            };

            yield return new PendingCharge()
            {
                SchemeName = "Another Cool Scheme",
                SchemeApprovalNumber = "WEEE/12345/01",
                ComplianceYear = 2016,
                TotalGBP = 32089.40m
            };

            yield return new PendingCharge()
            {
                SchemeName = "Biffa",
                SchemeApprovalNumber = "WEEE/54321/02",
                ComplianceYear = 2017,
                TotalGBP = 2500.10m
            };

            yield return new PendingCharge()
            {
                SchemeName = "Biffa",
                SchemeApprovalNumber = "WEEE/54321/02",
                ComplianceYear = 2016,
                TotalGBP = 2800.50m
            };
        }
    }
}