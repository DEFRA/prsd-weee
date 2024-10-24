﻿namespace EA.Weee.Api
{
    using Autofac;
    using Autofac.Integration.WebApi;
    using Core;
    using DataAccess;
    using DataAccess.Identity;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.HangfireServices;
    using EA.Weee.Email;
    using EA.Weee.Xml;
    using Identity;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Autofac;
    using RequestHandlers;
    using Security;
    using System.Web.Http;
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
            builder.RegisterModule(new SecurityModule());
            builder.RegisterModule(new ApiClientModule());

            // http://www.talksharp.com/configuring-autofac-to-work-with-the-aspnet-identity-framework-in-mvc-5
            builder.RegisterType<WeeeIdentityContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().As<UserManager<ApplicationUser>>().InstancePerRequest();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString;

            builder.RegisterType<SecurityEventDatabaseAuditor>()
                .WithParameter(new NamedParameter("connectionString", connectionString))
                .As<ISecurityEventAuditor>()
                .InstancePerRequest();

            builder.RegisterType<PaymentsJob>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentsService>().As<IPaymentsService>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}