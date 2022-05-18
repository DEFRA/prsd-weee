namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin.Obligations;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ObligationsController : ObligationsBaseController
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly BreadcrumbService breadcrumb;

        public ObligationsController(Func<IWeeeClient> apiClient,BreadcrumbService breadcrumb)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
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