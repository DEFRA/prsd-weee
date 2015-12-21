namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;
    using ViewModels.Charge;

    public class ChargeController : AdminController
    {
        private readonly IAppConfiguration configuration;
        private readonly BreadcrumbService breadcrumb;

        public ChargeController(
            IAppConfiguration configuration,
            BreadcrumbService breadcrumb)
        {
            this.configuration = configuration;
            this.breadcrumb = breadcrumb;
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
                    throw new NotImplementedException();

                case Activity.ManageIssuedCharges:
                    throw new NotImplementedException();

                case Activity.ViewInvoiceRunHistory:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}