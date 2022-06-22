namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    public abstract class AdminBreadcrumbBaseController : AdminController
    {
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;

        protected AdminBreadcrumbBaseController(BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.Breadcrumb = breadcrumb;
            this.Cache = cache;
        }

        protected async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            Breadcrumb.InternalOrganisation = await Cache.FetchOrganisationName(organisationId);
            Breadcrumb.InternalActivity = activity;
            Breadcrumb.OrganisationId = organisationId;
        }
    }
}
