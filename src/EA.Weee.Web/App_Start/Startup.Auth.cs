namespace EA.Weee.Web
{
    using System;
    using EA.Weee.Web.App_Start;
    using Infrastructure;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using Prsd.Core.Web.Mvc.Owin;
    using Services;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, IAppConfiguration config)
        {
            ReturnUrlMapping returnUrlMapping = new ReturnUrlMapping();
            returnUrlMapping.Add("/account/sign-out", null);
            returnUrlMapping.Add("/admin/account/sign-out", null);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = Constants.WeeeAuthType,
                LoginPath = new PathString("/account/sign-in"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(60),
                CookieName = EA.Prsd.Core.Web.Constants.CookiePrefix + Constants.WeeeAuthType,
                Provider = new WeeeCookieAuthenticationProvider(returnUrlMapping)
            });
        }
    }
}