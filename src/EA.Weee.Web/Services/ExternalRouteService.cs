namespace EA.Weee.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Uses the current HttpContext to resolve routes that are used externally
    /// to the WEEE website.
    /// </summary>
    public class ExternalRouteService : IExternalRouteService
    {
        private UrlHelper UrlHelper
        {
            get
            {
                return new UrlHelper(HttpContext.Current.Request.RequestContext);
            }
        }

        public string ActivateInternalUserAccountUrl
        {
            get
            {
                string protocol = UrlHelper.RequestContext.HttpContext.Request.Url.Scheme;
                return UrlHelper.Action("ActivateUserAccount", "Account", new { area = "Admin" }, protocol);
            }
        }

        public string ActivateExternalUserAccountUrl
        {
            get
            {
                string protocol = UrlHelper.RequestContext.HttpContext.Request.Url.Scheme;
                return UrlHelper.Action("ActivateUserAccount", "Account", null, protocol);
            }
        }
    }
}