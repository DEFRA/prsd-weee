﻿namespace EA.Weee.Web
{
    using System.Reflection;
    using System.Web;
    using Areas.AatfReturn.Attributes;
    using Areas.AatfReturn.ViewModels.Validation;
    using Authorization;
    using Autofac;
    using Autofac.Integration.Mvc;
    using EA.Weee.Core;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Search.Fuzzy;
    using EA.Weee.Core.Search.Simple;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FluentValidation;
    using Infrastructure;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mapper;
    using Requests.Base;
    using Security;

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

            // Register mappings
            builder.RegisterModule(new MappingModule());

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IMap<,>));

            // Register security module
            builder.RegisterModule(new SecurityModule());

            // Register caching
            builder.RegisterType<InMemoryCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<WeeeCache>()
                .As<IWeeeCache>()
                .As<ISearchResultProvider<ProducerSearchResult>>()
                .As<ISearchResultProvider<OrganisationSearchResult>>();

            // Breadcrumb
            builder.RegisterType<BreadcrumbService>().InstancePerRequest();

            // Authorization
            builder.RegisterType<WeeeAuthorization>().As<IWeeeAuthorization>();

            // External route resolution
            builder.RegisterType<ExternalRouteService>().As<IExternalRouteService>().InstancePerRequest();

            // We're going to use the simple producer searcher.
            builder.RegisterType<SimpleProducerSearcher>()
                .As<ISearcher<ProducerSearchResult>>()
                .InstancePerRequest();

            // We're going to use the fuzzy organisation searcher.
            builder.RegisterType<FuzzyOrganisationSearcher>()
                .As<ISearcher<OrganisationSearchResult>>()
                .InstancePerRequest();

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces().InstancePerRequest();

            // AATF Return Validators
            builder.RegisterType<NonObligatedValuesViewModelValidatorWrapper>().As<INonObligatedValuesViewModelValidatorWrapper>();
            builder.RegisterType<SelectReportOptionsViewModelValidatorWrapper>().As<ISelectReportOptionsViewModelValidatorWrapper>();

            // AATF View Model Mapping Utilties
            builder.RegisterType<CategoryValueTotalCalculator>().As<ICategoryValueTotalCalculator>();
            builder.RegisterType<TonnageUtilities>().As<ITonnageUtilities>();
            builder.RegisterType<AddressUtilities>().As<IAddressUtilities>();
            builder.RegisterType<ReturnsOrdering>().As<IReturnsOrdering>();

            builder.RegisterType<ValidateOrganisationActionFilterAttribute>().PropertiesAutowired();

            return builder.Build();
        }
    }
}