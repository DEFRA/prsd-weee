namespace EA.Weee.Web.Areas.Test.Controllers
{
    using Filters;
    using Security;
    using System.Web.Mvc;

    [AuthorizeClaimsAttribute(Claims.CanAccessInternalArea)]
    public abstract class TestControllerBase : Controller
    {
    }
}