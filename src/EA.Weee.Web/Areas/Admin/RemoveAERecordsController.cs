namespace EA.Weee.Web.Areas.Admin
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using System;
    using System.Web.Mvc;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public class RemoveAERecordsController : AdminController
    {
        private readonly Func<IWeeeClient> apiClient;
        private const int pageSize = 10;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public RemoveAERecordsController(Func<IWeeeClient> apiClient, BreadcrumbService breadcrumb, IMapper mapper)
        {
            this.apiClient = apiClient;
            this.breadcrumb = breadcrumb;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}