namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using System.Web.Mvc;
    using Core;
    using Filters;

    [AuthorizeClaims(Claims.CanAccessInternalArea)]
    public abstract class AdminController : Controller
    {
    }
}