namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    [ValidatePcsObligationsEnabled]
    public abstract class ObligationsBaseController : AdminBreadcrumbBaseController
    {
        protected ObligationsBaseController(BreadcrumbService breadcrumb, IWeeeCache cache) : base(breadcrumb, cache)
        {
        }
    }
}