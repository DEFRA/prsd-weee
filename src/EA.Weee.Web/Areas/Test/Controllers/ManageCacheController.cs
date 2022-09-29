namespace EA.Weee.Web.Areas.Test.Controllers
{
    using EA.Weee.Web.Services.Caching;
    using Services;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize]
    public class ManageCacheController : TestControllerBase
    {
        private readonly IWeeeCache cache;
        private readonly BreadcrumbService breadcrumb;

        public ManageCacheController(
            IWeeeCache cache,
            BreadcrumbService breadcrumb)
        {
            this.cache = cache;
            this.breadcrumb = breadcrumb;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            breadcrumb.TestAreaActivity = "Manage cache";
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            cache.Clear();

            return View("CacheCleared");
        }
    }
}