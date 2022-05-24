namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin.Obligations;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using Services;
    using Services.Caching;
    using System;
    using EA.Weee.Web.Infrastructure;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;

    public class ObligationsController : ObligationsBaseController
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;

        public ObligationsController(IAppConfiguration configuration, BreadcrumbService breadcrumb, IWeeeCache cache, Func<IWeeeClient> apiClient) : base(breadcrumb, cache)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
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
            return View("SelectAuthority", new SelectAuthorityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectAuthority(SelectAuthorityViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("UploadObligations", new UploadObligationsViewModel(model.SelectedAuthority.Value));
            }
            else
            {
                return View("SelectAuthority", model);
            }
        }

        [HttpGet]
        public ActionResult UploadObligations(CompetentAuthority authority)
        {
            var model = new UploadObligationsViewModel(authority);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadObligations(UploadObligationsViewModel model)
        {
            ViewBag.TriggerDownload = ModelState.IsValid;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadTemplate(CompetentAuthority authority)
        {
            using (var client = apiClient())
            {
                var fileData = await client.SendAsync(this.User.GetAccessToken(), new GetPcsObligationsCsv(authority));

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }
    }
}