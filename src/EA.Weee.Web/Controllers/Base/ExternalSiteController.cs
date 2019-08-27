namespace EA.Weee.Web.Controllers.Base
{
    using Filters;
    using Security;
    using System.Web.Mvc;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public abstract class ExternalSiteController : Controller
    {
    }
}