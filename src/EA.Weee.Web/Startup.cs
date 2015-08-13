using EA.Weee.Web;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EA.Weee.Web
{
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Infrastructure;
    using Owin;
    using Services;
    using Thinktecture.IdentityModel.Client;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new ConfigurationService();

            var builder = new ContainerBuilder();
            builder.Register(c => configuration).As<ConfigurationService>().SingleInstance();
            builder.Register(c => configuration.CurrentConfiguration).As<IAppConfiguration>().SingleInstance();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();

            var container = AutofacBootstrapper.Initialize(builder);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Must register Autofac middleware FIRST!
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            ConfigureAuth(app, configuration.CurrentConfiguration);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = JwtClaimTypes.Subject;
            AntiForgeryConfig.RequireSsl = true;
            AntiForgeryConfig.CookieName = Prsd.Core.Web.Constants.CookiePrefix + Constants.AntiForgeryCookieName;

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}