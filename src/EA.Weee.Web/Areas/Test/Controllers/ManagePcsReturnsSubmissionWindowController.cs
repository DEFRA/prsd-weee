namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Infrastructure;
    using ViewModels.ManagePcsReturnsSubmissionWindow;
    using Weee.Requests.Test;

    [Authorize]
    public class ManagePcsReturnsSubmissionWindowController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;

        public ManagePcsReturnsSubmissionWindowController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View(new SettingsModel());
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
                    CurrentComplianceYear = settings.CurrentComplianceYear,
                    FixCurrentQuarterAndComplianceYear = settings.FixCurrentQuarterAndComplianceYear,
                    SelectedQuarter = settings.SelectedQuarter
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