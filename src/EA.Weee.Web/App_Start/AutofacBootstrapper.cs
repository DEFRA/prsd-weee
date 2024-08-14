﻿namespace EA.Weee.Web
{
    using Areas.Aatf.Helpers;
    using Areas.Aatf.Mappings.Filters;
    using Areas.AatfReturn.Attributes;
    using Areas.Scheme.Requests;
    using Authorization;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Core.Shared;
    using EA.Weee.Core;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Search;
    using EA.Weee.Core.Search.Fuzzy;
    using EA.Weee.Core.Search.Simple;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using EA.Weee.Web.Hangfire;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FluentValidation;
    using Infrastructure;
    using Infrastructure.PDF;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mapper;
    using Requests.Base;
    using Security;
    using Serilog;
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
            builder.RegisterModule(new CoreModule());

            // Register request creators
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IRequestCreator<,>));

            // Register mappings
            builder.RegisterModule(new MappingModule());

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IMap<,>));

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IAatfDataFilter<,>));

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

            builder.RegisterType<CategoryValueTotalCalculator>().As<ICategoryValueTotalCalculator>();
            builder.RegisterType<AddressUtilities>().As<IAddressUtilities>();
            builder.RegisterType<ReturnsOrdering>().As<IReturnsOrdering>();
            builder.RegisterType<TonnageUtilities>().As<ITonnageUtilities>();
            builder.RegisterType<FacilityViewModelBaseValidatorWrapper>().As<IFacilityViewModelBaseValidatorWrapper>();
            builder.RegisterType<ValidateOrganisationActionFilterAttribute>().PropertiesAutowired();
            builder.RegisterType<SessionService>().As<ISessionService>();
            builder.RegisterType<HttpContextService>().As<IHttpContextService>();
            builder.RegisterType<TransferEvidenceNoteRequestCreator>().As<ITransferEvidenceRequestCreator>();
            builder.RegisterType<AatfEvidenceHelper>().As<IAatfEvidenceHelper>();

            builder.RegisterType<MvcTemplateExecutor>().As<IMvcTemplateExecutor>();
            builder.RegisterType<PdfDocumentProvider>().As<IPdfDocumentProvider>();

            builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
            builder.RegisterType<MyJob>();

            builder.Register(c => Log.Logger)
                .As<ILogger>()
                .SingleInstance();

            return builder.Build();
        }
    }
}