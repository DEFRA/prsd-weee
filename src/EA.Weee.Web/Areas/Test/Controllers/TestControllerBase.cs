namespace EA.Weee.Web.Areas.Test.Controllers
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeInternalClaims(Claims.CanAccessInternalArea)]
    public abstract class TestControllerBase : Controller
    {
    }
}