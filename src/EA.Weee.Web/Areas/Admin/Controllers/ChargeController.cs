namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;

    public class ChargeController : AdminController
    {
        [HttpGet]
        public ActionResult SelectAA()
        {
            return View();
        }
    }
}