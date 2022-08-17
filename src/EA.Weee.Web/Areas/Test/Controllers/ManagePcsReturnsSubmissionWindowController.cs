namespace EA.Weee.Web.Areas.Test.Controllers
{
    using Api.Client;
    using Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Services.Caching;
    using ViewModels.ManagePcsReturnsSubmissionWindow;
    using Weee.Requests.Test;

    [Authorize]
    public class ManagePcsReturnsSubmissionWindowController : TestControllerBase
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeCache cache;

        public ManagePcsReturnsSubmissionWindowController(Func<IWeeeClient> apiClient, IWeeeCache cache)
        {
            this.apiClient = apiClient;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult> Settings()
        {
            using (var client = apiClient())
            {
                var settings = await client.SendAsync(User.GetAccessToken(), new GetPcsSubmissionWindowSettings());
                return View(new SettingsModel
                {
                    CurrentDate = settings.CurrentDate,
                    FixCurrentDate = settings.FixCurrentDate
                });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Settings(SettingsModel settings)
        {
            if (!ModelState.IsValid)
            {
                return View(settings);
            }

            using (var client = apiClient())
            {
                await client.SendAsync(User.GetAccessToken(), new UpdatePcsSubmissionWindowSettings
                {
                    FixCurrentDate = settings.FixCurrentDate,
                    CurrentDate = settings.CurrentDate
                });
            }

            return RedirectToAction("SettingsUpdated", "ManagePcsReturnsSubmissionWindow");
        }

        [HttpGet]
        public ActionResult SettingsUpdated()
        {
            return View();
        }
    }
}