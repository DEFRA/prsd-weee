namespace EA.Weee.Web.Areas.Test.Controllers
{
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Mvc;

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