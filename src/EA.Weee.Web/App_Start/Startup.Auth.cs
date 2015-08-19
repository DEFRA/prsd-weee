﻿namespace EA.Weee.Web
{
    using EA.Weee.Web.App_Start;
    using Infrastructure;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using Prsd.Core.Web.Mvc.Owin;
    using Services;
    using System;
    using System.Threading.Tasks;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, IAppConfiguration config)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = Constants.WeeeAuthType,
                LoginPath = new PathString("/Account/Login"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                CookieName = EA.Prsd.Core.Web.Constants.CookiePrefix + Constants.WeeeAuthType,
                Provider = new WeeeCookieAuthenticationProvider()
            });
        }
    }
}