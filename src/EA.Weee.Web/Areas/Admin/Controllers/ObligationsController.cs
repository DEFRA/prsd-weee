namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;
    using Services.Caching;
    using System;
    using System.Security.Claims;
    using System.Web.Mvc;

    public class ObligationsController : ObligationsBaseController
    {
        private readonly IAppConfiguration configuration;

        public ObligationsController(IAppConfiguration configuration, BreadcrumbService breadcrumb, IWeeeCache cache) : base(breadcrumb, cache)
        {
            this.configuration = configuration;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!configuration.EnablePCSObligations)
            {
                throw new InvalidOperationException("PCS Obligations is not enabled.");
            }

            Breadcrumb.InternalActivity = "Manage PCS obligations";

            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}