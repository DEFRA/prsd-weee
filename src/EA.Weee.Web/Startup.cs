using EA.Weee.Web;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EA.Weee.Web
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using EA.Weee.Web.App_Start;
    using FluentValidation.Mvc;
    using Hangfire;
    using IdentityModel;
    using Infrastructure;
    using Owin;
    using Services;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using global::Hangfire;
    using global::Hangfire.SqlServer;
    using Serilog;
    using Serilog.Sinks.MSSqlServer;

    public partial class Startup
    {
        public static string ApplicationVersion { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var configuration = new ConfigurationService();

            var builder = new ContainerBuilder();
            builder.Register(c => configuration).As<ConfigurationService>().SingleInstance();
            builder.Register(c => configuration.CurrentConfiguration).As<IAppConfiguration>().SingleInstance();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();

            Log.Logger = new LoggerConfiguration()
                            .WriteTo.MSSqlServer(
                            connectionString: System.Configuration.ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString,
                            sinkOptions: new MSSqlServerSinkOptions
                            {
                                SchemaName = "Logging",
                                TableName = "Logs",
                                AutoCreateSqlTable = true
                            })
                            .CreateLogger();

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

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, configuration.CurrentConfiguration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(WeeeRequiredAttributeAdapter));

            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            FluentValidationModelValidatorProvider.Configure();

            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            GlobalConfiguration.Configuration
                .UseAutofacActivator(container)
                .UseSqlServerStorage("Weee.DefaultConnection");

            //BackgroundJob.Enqueue<MyJob>(job => job.Execute());

            //Log.Logger = new LoggerConfiguration()
            //   .WriteTo.Elmah()
            //   .CreateLogger();
            //var options = new SqlServerStorageOptions
            //{
            //    PrepareSchemaIfNecessary = false
            //};
            HangfireBootstrapper.Instance.Start();
            //GlobalConfiguration.Configuration.UseSqlServerStorage("<name or connection string>", options);
            var user2 = DependencyResolver.Current.GetService<IMyService>();
            var user = DependencyResolver.Current.GetService<MyJob>();

            RecurringJob.AddOrUpdate<MyJob>("my-recurring-job", job => user.Execute(), "* * * * *");
        }

        protected void Application_End()
        {
            HangfireBootstrapper.Instance.Stop();
        }
    }
}