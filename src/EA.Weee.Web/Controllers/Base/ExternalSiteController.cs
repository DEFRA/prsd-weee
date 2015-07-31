namespace EA.Weee.Web.Controllers.Base
{
    using System.Web.Mvc;
    using Core;
    using Filters;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public abstract class ExternalSiteController : Controller
    {
    }
}