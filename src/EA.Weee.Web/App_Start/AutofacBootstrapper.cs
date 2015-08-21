namespace EA.Weee.Web
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Modules;
    using Requests.Base;
    using System.Linq;
    using System.Reflection;

    public class AutofacBootstrapper
    {
        public static IContainer Initialize(ContainerBuilder builder)
        {
            // Register all controllers
            builder.RegisterControllers(typeof(Startup).Assembly);

            // Register model binders
            builder.RegisterModelBinders(typeof(Startup).Assembly);
            builder.RegisterModelBinderProvider();

            // Register all HTTP abstractions
            builder.RegisterModule<AutofacWebTypesModule>();

            // Allow property injection in views
            builder.RegisterSource(new ViewRegistrationSource());

            // Allow property injection in action filters
            builder.RegisterFilterProvider();

            // Register all Autofac specific IModule implementations
            builder.RegisterAssemblyModules(typeof(Startup).Assembly);
            
            // Register request creators
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IRequestCreator<,>));

            // Register caching
            builder.RegisterType<InMemoryCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<WeeeCache>().As<IWeeeCache>();

            // Breadcrumb
            builder.RegisterType<BreadcrumbService>().InstancePerRequest();

            return builder.Build();
        }
    }
}