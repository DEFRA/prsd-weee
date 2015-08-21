namespace EA.Weee.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class BreadcrumbService
    {
        public string InternalActivity { get; set; }
        public string ExternalActivity { get; set; }
        public string Organsiation { get; set; }
        public string User { get; set; }
    }
}