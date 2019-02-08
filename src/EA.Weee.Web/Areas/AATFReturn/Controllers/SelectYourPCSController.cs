namespace EA.Weee.Web.Areas.AatfReturn.Controllers
{
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;


    public class SelectYourPCSController : ExternalSiteController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new SelectYourPCSViewModel();
            return View("Index", viewModel);
        }
    }
}