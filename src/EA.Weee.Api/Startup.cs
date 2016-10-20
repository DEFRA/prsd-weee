using EA.Weee.Api;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EA.Weee.Api
{
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Elmah.Contrib.WebApi;
    using IdentityServer3.AccessTokenValidation;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Logging;
    using IdSrv;
    using Microsoft.Owin.Security.DataProtection;
    using Owin;
    using Services;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var configurationService = new ConfigurationService();
#if DEBUG
            LogProvider.SetCurrentLogProvider(new DebugLogProvider());
            config.Services.Add(typeof(IExceptionLogger), new DebugExceptionLogger());
#else
            LogProvider.SetCurrentLogProvider(new ElmahLogProvider());
#endif
            // Autofac
            var builder = new ContainerBuilder();
            builder.Register(c => app.GetDataProtectionProvider()).InstancePerRequest();
            builder.Register(c => configurationService).As<ConfigurationService>().SingleInstance();
            builder.Register(c => configurationService.CurrentConfiguration).As<AppConfiguration>().SingleInstance();

            var container = AutofacBootstrapper.Initialize(builder, config);

            // Web API
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.Filters.Add(new ElmahHandleErrorApiAttribute());
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseIdentityServer(GetIdentityServerOptions(app, configurationService.CurrentConfiguration));

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = configurationService.CurrentConfiguration.SiteRoot,
                RequiredScopes = new[] { "api1" }
            });

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        private static IdentityServerOptions GetIdentityServerOptions(IAppBuilder app, AppConfiguration config)
        {
            var factory = Factory.Configure(config);
            factory.ConfigureUserService(app);

            return new IdentityServerOptions
            {
                Factory = factory,
                RequireSsl = false,
                EventsOptions = new EventsOptions()
                {
                    RaiseSuccessEvents = true,
                    RaiseFailureEvents = true
                }
            };
        }
    }
}