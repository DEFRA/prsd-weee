namespace EA.Weee.Web
{
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Modules;
    using Requests.Base;

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

            // Register file converter module
            builder.RegisterModule<FileConverterModule>();
            
            // Register request creators
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IRequestCreator<,>));

            return builder.Build();
        }
    }
}