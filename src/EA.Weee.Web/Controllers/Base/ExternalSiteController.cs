namespace EA.Weee.Web.Controllers.Base
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public abstract class ExternalSiteController : Controller
    {
    }
}