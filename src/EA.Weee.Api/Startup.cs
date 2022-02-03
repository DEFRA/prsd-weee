using EA.Weee.Api;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EA.Weee.Api
{
    using System.Linq;
    using System.Net;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Elmah.Contrib.WebApi;
    using IdentityServer3.AccessTokenValidation;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Logging;
    using IdSrv;
    using Microsoft.Owin.Security.DataProtection;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using Serilog;
    using Services;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Filters;
    using Infrastructure.Infrastructure;
    using LoggerConfigurationExtensions = Logging.LoggerConfigurationExtensions;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var configurationService = new ConfigurationService();
#if DEBUG
            Log.Logger = LoggerConfigurationExtensions.Debug(new LoggerConfiguration()
                    .WriteTo)
                .CreateLogger();

            config.Services.Add(typeof(IExceptionLogger), new DebugExceptionLogger());
#else
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elmah()
                .CreateLogger();
#endif
            // Autofac
            var builder = new ContainerBuilder();
            builder.Register(c => app.GetDataProtectionProvider()).InstancePerRequest();
            builder.Register(c => configurationService).As<ConfigurationService>().SingleInstance();
            builder.Register(c => configurationService.CurrentConfiguration).As<AppConfiguration>().SingleInstance();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register(c => Log.Logger).As<ILogger>().SingleInstance();
            builder.RegisterType<ElmahSqlLogger>().AsSelf().InstancePerRequest();

            var container = AutofacBootstrapper.Initialize(builder, config);
            System.Net.ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Web API
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.Filters.AddRange(new FilterConfig(configurationService.CurrentConfiguration).Collection);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true };

            app.UseIdentityServer(GetIdentityServerOptions(app, configurationService.CurrentConfiguration));

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = configurationService.CurrentConfiguration.SiteRoot,
                RequiredScopes = new[] { "api1", "api2" },
                ValidationMode = ValidationMode.ValidationEndpoint
            });

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseClaimsTransformation(ClaimsTransformationOptionsFactory.Create());
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
                EnableWelcomePage = false,
                EventsOptions = new EventsOptions()
                {
                    RaiseSuccessEvents = true,
                    RaiseFailureEvents = true,
                },
            };
        }
    }
}