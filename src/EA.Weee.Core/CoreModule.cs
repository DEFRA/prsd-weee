﻿namespace EA.Weee.Core
{
    using Autofac;
    using Configuration;
    using Shared;
    using Validation;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register the helper classes
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Namespace.Contains("Helpers"))
                .AsImplementedInterfaces();

            builder.RegisterType<NoFormulaeExcelSanitizer>().As<IExcelSanitizer>();

            builder.RegisterType<CsvWriterFactory>().SingleInstance();

            builder.RegisterGeneric(typeof(CsvWriter<>))
                .As(typeof(ICsvWriter<>))
                .InstancePerDependency();

            builder.RegisterType<ConfigurationManagerWrapper>().As<IConfigurationManagerWrapper>();

            builder.Register(c => c.Resolve<IConfigurationManagerWrapper>().TestInternalUserEmailDomains)
                .As<ITestUserEmailDomains>();

            builder.RegisterType<PasteProcessor>().As<IPasteProcessor>();
            builder.RegisterType<FileHelper>().As<IFileHelper>();
            builder.RegisterType<TonnageValueValidator>().As<ITonnageValueValidator>();
            builder.RegisterType<AddressUtilities>().As<IAddressUtilities>();
        }
    }
}
