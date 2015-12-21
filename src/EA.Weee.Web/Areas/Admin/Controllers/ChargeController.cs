namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;

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
        public ActionResult SelectAA()
        {
            return View();
        }
    }
}