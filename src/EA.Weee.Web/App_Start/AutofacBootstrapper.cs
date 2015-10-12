﻿namespace EA.Weee.Web
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using EA.Weee.Core;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using Modules;
    using Requests.Base;
    using System.Linq;
    using System.Reflection;
    using Authorization;
    using EA.Weee.Core.Search;

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
            builder.RegisterModule(new CoreModule());

            // Register request creators
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IRequestCreator<,>));

            // Register caching
            builder.RegisterType<InMemoryCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<WeeeCache>()
                .As<IWeeeCache>()
                .As<IProducerSearchResultProvider>()
                .As<IOrganisationSearchResultProvider>();

            // Breadcrumb
            builder.RegisterType<BreadcrumbService>().InstancePerRequest();

            // Authorization
            builder.RegisterType<WeeeAuthorization>().As<IWeeeAuthorization>();

            // External route resolution
            builder.RegisterType<ExternalRouteService>().As<IExternalRouteService>().InstancePerRequest();

            // We're going to use the simple producer searcher.
            builder.RegisterType<SimpleProducerSearcher>().As<IProducerSearcher>().InstancePerRequest();

            // We're going to use the simple organisation searcher.
            builder.RegisterType<SimpleOrganisationSearcher>().As<IOrganisationSearcher>().InstancePerRequest();

            return builder.Build();
        }
    }
}