namespace EA.Weee.Web.ViewModels.Shared
{
    using EA.Weee.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;

    public class TitleViewModel
    {
        public IPrincipal User { get; set; }
        public BreadcrumbService Breadcrumb { get; set; }
    }
}