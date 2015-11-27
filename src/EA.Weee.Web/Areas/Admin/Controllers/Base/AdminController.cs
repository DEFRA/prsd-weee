namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using Core;
    using Filters;
    using System.Web.Mvc;

    [AuthorizeClaims(Claims.CanAccessInternalArea)]
    public abstract class AdminController : Controller
    {
    }
}