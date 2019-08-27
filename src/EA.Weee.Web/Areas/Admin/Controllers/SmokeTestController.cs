namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [AllowAnonymous]
    public class SmokeTestController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public SmokeTestController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var client = apiClient())
            {
                var info = await client.SmokeTest.PerformTest();
                return Json(info, JsonRequestBehavior.AllowGet);
            }
        }
    }
}