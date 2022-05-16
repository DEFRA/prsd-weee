namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using EA.Weee.Security;
    using EA.Weee.Web.Filters;

    [AuthorizeInternalClaims(Claims.InternalAdmin)]
    public abstract class ObligationsBaseController : AdminController
    {
    }
}