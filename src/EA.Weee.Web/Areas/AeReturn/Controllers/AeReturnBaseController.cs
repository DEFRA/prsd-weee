namespace EA.Weee.Web.Areas.AeReturn.Controllers
{
    using System.Web.Mvc;
    using Web.Controllers.Base;

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public abstract class AeReturnBaseController : ExternalSiteController
    {
    }
}