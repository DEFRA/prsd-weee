namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Web.Services.Caching;
    using Services;

    [Authorize]
    public class ManageCacheController : Controller
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
        public async Task<ActionResult> Index(FormCollection form)
        {
            await cache.InvalidateProducerSearch();

            return View("CacheCleared");
        }
   }
}