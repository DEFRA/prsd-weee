namespace EA.Weee.Web
{
    using Infrastructure;
    using Microsoft.Owin;
    using Owin;
    using Prsd.Core.Web.Mvc.Owin;
    using Services;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, AppConfiguration config)
        {
            app.UseCookieAuthentication(new PrsdCookieAuthenticationOptions(
                authenticationType: Constants.WeeeAuthType,
                apiUrl: config.ApiUrl,
                apiSecret: config.ApiSecret)
            {
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}