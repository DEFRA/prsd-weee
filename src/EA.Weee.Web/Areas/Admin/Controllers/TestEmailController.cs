namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.TestEmail;
    using Weee.Requests.Admin;

    public class TestEmailController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;

        public TestEmailController(Func<IWeeeClient> apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(TestEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = apiClient())
            {
                var result = await client.SendAsync(User.GetAccessToken(), new SendTestEmail(model.EmailTo));

                if (result)
                {
                    return RedirectToAction("Success");
                }
            }

            ModelState.AddModelError(string.Empty, "An error occurred while sending the email");
            return View(model);
        }

        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }
    }
}