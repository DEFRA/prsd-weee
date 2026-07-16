namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using EA.Weee.Web.Areas.Admin.ViewModels.ArchiveData;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Services;
    using System.Web.Mvc;

    public class ArchiveDataController : Controller
    {
        private readonly BreadcrumbService breadcrumb;

        public ArchiveDataController(BreadcrumbService breadcrumb)
        {
            this.breadcrumb = breadcrumb;
        }

        // GET: Admin/ArchiveData
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChooseArchiveType()
        {
            SetBreadcrumb();

            var model = new ChooseArchiveTypeViewModel();

            return View(model);
        }

        protected void SetBreadcrumb()
        {
            breadcrumb.InternalActivity = InternalUserActivity.ArchiveData;
        }
    }
}