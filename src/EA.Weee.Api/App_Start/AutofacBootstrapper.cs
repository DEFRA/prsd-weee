namespace EA.Weee.Api
{
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Core;
    using Core.Logging;
    using DataAccess;
    using DataAccess.Identity;
    using EA.Weee.Email;
    using EA.Weee.Xml;
    using Identity;
    using IdSrv;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Autofac;
    using RequestHandlers;
    using XmlValidation;

    public class AutofacBootstrapper
    {
        public static IContainer Initialize(ContainerBuilder builder, HttpConfiguration config)
        {
            // Register all controllers
            builder.RegisterApiControllers(typeof(Startup).Assembly);

            // Register Autofac filter provider
            builder.RegisterWebApiFilterProvider(config);

            // Register model binders
            builder.RegisterWebApiModelBinders(typeof(Startup).Assembly);
            builder.RegisterWebApiModelBinderProvider();

            // Register all Autofac specific IModule implementations
            builder.RegisterAssemblyModules(typeof(Startup).Assembly);
            builder.RegisterAssemblyModules(typeof(AutofacMediator).Assembly);
            builder.RegisterModule(new RequestHandlerModule());
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new EntityFrameworkModule());
            builder.RegisterModule(new EmailModule());
            builder.RegisterModule(new XmlValidationModule());
            builder.RegisterModule(new EventDispatcherModule());
            builder.RegisterModule(new XmlModule());

            // http://www.talksharp.com/configuring-autofac-to-work-with-the-aspnet-identity-framework-in-mvc-5
            builder.RegisterType<WeeeIdentityContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().As<UserManager<ApplicationUser>>().InstancePerRequest();
            builder.RegisterType<ElmahLogger>().As<ILogger>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString;

            builder.RegisterType<SecurityEventDatabaseAuditor>()
                .WithParameter(new NamedParameter("connectionString", connectionString))
                .As<ISecurityEventAuditor>()
                .InstancePerRequest();

            return builder.Build();
        }
    }
}