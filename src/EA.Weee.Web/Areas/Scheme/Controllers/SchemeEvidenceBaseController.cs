namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Controllers.Base;
    using Services;
    using Services.Caching;

    [ValidatePcsEvidenceEnabled]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public abstract class SchemeEvidenceBaseController : ExternalSiteController
    {
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;

        protected SchemeEvidenceBaseController(BreadcrumbService breadcrumb, IWeeeCache cache)
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