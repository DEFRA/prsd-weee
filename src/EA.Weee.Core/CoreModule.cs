namespace EA.Weee.Core
{
    using Autofac;
    using Autofac.Core;
    using EA.Weee.Core.Configuration;
    using EA.Weee.Core.Shared;
    using XmlBusinessValidation;

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

            // XML rules
            builder.RegisterType<RuleSelector>().As<IRuleSelector>();

            builder.RegisterType<ConfigurationManagerWrapper>().As<IConfigurationManagerWrapper>();

            builder.Register(c => c.Resolve<IConfigurationManagerWrapper>().TestInternalUserEmailDomains)
                .As<ITestInternalUserEmailDomains>();
        }
    }
}
