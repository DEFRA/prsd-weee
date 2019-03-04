namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using System.Web.Mvc;
    using Web.Controllers.Base;

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public abstract class AatfReturnBaseController : ExternalSiteController
    {
    }
}