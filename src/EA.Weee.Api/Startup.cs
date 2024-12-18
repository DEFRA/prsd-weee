﻿using System.Collections.Generic;
using EA.Weee.Api;
using EA.Weee.Api.App_Start;
using Hangfire;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EA.Weee.Api
{
    using Autofac;
    using Autofac.Integration.WebApi;
    using EA.Weee.Api.HangfireServices;
    using EA.Weee.Core.Configuration;
    using EA.Weee.Core.Configuration.Logging;
    using Elmah.Contrib.WebApi;
    using IdentityServer3.AccessTokenValidation;
    using IdentityServer3.Core.Configuration;
    using IdSrv;
    using Infrastructure.Infrastructure;
    using Microsoft.Owin.Security.DataProtection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using Serilog;
    using Services;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
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

            LoggerConfig.ConfigureSerilogWithSqlServer();
            builder.Register(c => Log.Logger).As<ILogger>().SingleInstance();

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

            Hangfire.GlobalConfiguration.Configuration
                .UseAutofacActivator(container)
                .UseSqlServerStorage("Weee.DefaultConnection");

            HangfireBootstrapper.Instance.Start();

            if (configurationService.CurrentConfiguration.GovUkPayMopUpJobEnabled)
            {
                RecurringJob.AddOrUpdate<PaymentsJob>("payments-job", job => job.Execute(Guid.NewGuid()), configurationService.CurrentConfiguration.GovUkPayMopUpJobSchedule);
            }

            DiagnosticSourceDisposer.DisposeDiagnosticSourceEventSource();
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