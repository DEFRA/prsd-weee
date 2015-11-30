namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Web.Services.Caching;

    [Authorize]
    public class ManageCacheController : Controller
    {
        private readonly IWeeeCache cache;

        public ManageCacheController(IWeeeCache cache)
        {
            this.cache = cache;
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