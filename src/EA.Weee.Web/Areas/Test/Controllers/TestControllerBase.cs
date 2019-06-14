namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaimsAttribute(Claims.CanAccessInternalArea)]
    public abstract class TestControllerBase : Controller
    {
    }
}