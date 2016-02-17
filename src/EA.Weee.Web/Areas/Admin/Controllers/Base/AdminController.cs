namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaims(Claims.CanAccessInternalArea)]
    public abstract class AdminController : Controller
    {
    }
}