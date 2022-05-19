namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;
    using Services.Caching;
    using System;
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
        public ActionResult SelectAuthority()
        {
            return View("SelectAuthority", new ViewModels.Obligations.SelectAuthorityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Holding(ViewModels.Obligations.SelectAuthorityViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Index", new ViewModels.Obligations.SelectAuthorityViewModel());
            }
            else
            {
                return View("SelectAuthority", model);
            }
        }
    }
}