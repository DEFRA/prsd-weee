namespace EA.Weee.Core
{
    using Autofac;
    using Configuration;
    using Shared;

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

            builder.RegisterType<ConfigurationManagerWrapper>().As<IConfigurationManagerWrapper>();

            builder.Register(c => c.Resolve<IConfigurationManagerWrapper>().TestInternalUserEmailDomains)
                .As<ITestUserEmailDomains>();
        }
    }
}
