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

    public class ObligationsController : ObligationsBaseController
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;

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
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(Guid authorityId)
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> DownloadTemplate(Guid authorityId)
        {
            using (var client = apiClient())
            {
                var fileData = await client.SendAsync(this.User.GetAccessToken(), new GetPcsObligationsCsv(authorityId));

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }
    }
}