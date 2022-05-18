namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Threading.Tasks;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    [ValidatePcsObligationsEnabled]
    public abstract class ObligationsBaseController : AdminController
    {
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected ObligationsBaseController(BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.Breadcrumb = breadcrumb;
            this.Cache = cache;
        }

        protected async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            Breadcrumb.ExternalOrganisation = await Cache.FetchOrganisationName(organisationId);
            Breadcrumb.ExternalActivity = activity;
            Breadcrumb.OrganisationId = organisationId;
        }
    }
}