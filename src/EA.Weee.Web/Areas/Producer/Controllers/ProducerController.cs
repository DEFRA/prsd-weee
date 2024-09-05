namespace EA.Weee.Web.Areas.Producer.Controllers
{
    using EA.Weee.Core;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Controllers.Base;
    using System.Web.Mvc;

    [AuthorizeRouteClaims(WeeeClaimTypes.DirectRegistrantAccess)]
    public class ProducerController : ExternalSiteController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}