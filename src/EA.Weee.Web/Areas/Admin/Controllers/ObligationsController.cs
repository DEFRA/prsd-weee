namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using Api.Client;
    using Base;
    using Core.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using Infrastructure;
    using Mappings.ToViewModel;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Admin.Obligation;
    using Extensions;
    using ViewModels.Obligations;

    public class ObligationsController : ObligationsBaseController
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IMapper mapper;

        public ObligationsController(IAppConfiguration configuration, BreadcrumbService breadcrumb, IWeeeCache cache, Func<IWeeeClient> apiClient, IMapper mapper) : base(breadcrumb, cache)
        {
            this.configuration = configuration;
            this.apiClient = apiClient;
            this.mapper = mapper;
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
                return RedirectToAction("UploadObligations", new { authority = model.SelectedAuthority.Value});
            }
            
            return View("SelectAuthority", model);
        }

        [HttpGet]
        public async Task<ActionResult> UploadObligations(CompetentAuthority authority, Guid? id)
        {
            using (var client = apiClient())
            {
                SchemeObligationUploadData data = null;
                if (id.HasValue)
                {
                    data = await client.SendAsync(User.GetAccessToken(), new GetSchemeObligationUpload(id.Value));
                }

                var model = mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(new UploadObligationsViewModelMapTransfer()
                {
                    CompetentAuthority = authority,
                    UploadData = data
                });

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadObligations(UploadObligationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = apiClient())
                {
                    var request = mapper.Map<UploadObligationsViewModel, SubmitSchemeObligation>(model);

                    var result = await client.SendAsync(User.GetAccessToken(), request);

                    return RedirectToAction("UploadObligations", new { authority = model.Authority, id = result });
                }
            }

            if (ModelState.HasErrorForProperty<UploadObligationsViewModel, HttpPostedFileBase>(m => m.File))
            {
                model.DisplaySelectFileError = true;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadTemplate(CompetentAuthority authority)
        {
            using (var client = apiClient())
            {
                var fileData = await client.SendAsync(User.GetAccessToken(), new GetPcsObligationsCsv(authority));

                var data = new UTF8Encoding().GetBytes(fileData.FileContent);
                return File(data, "text/csv", CsvFilenameFormat.FormatFileName(fileData.FileName));
            }
        }
    }
}