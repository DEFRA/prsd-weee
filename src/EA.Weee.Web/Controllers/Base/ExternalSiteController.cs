namespace EA.Weee.Web.Controllers.Base
{
    using Core;
    using Filters;
    using System.Web.Mvc;

    [AuthorizeClaims(Claims.CanAccessExternalArea)]
    public abstract class ExternalSiteController : Controller
    {
    }
}