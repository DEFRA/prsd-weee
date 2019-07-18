namespace EA.Weee.Sroc.Migration.Autofac
{
    using global::Autofac;
    using DataAccess;
    using DataAccess.DataAccess;
    using OverrideImplementations;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xml.MemberRegistration;
    
    public class MigrationRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .AsNamedClosedTypesOf(typeof(IRequestHandler<,>), t => "request_handler");

            builder.RegisterAssemblyTypes()
                .AsClosedTypesOf(typeof(IRequest<>))
                .AsImplementedInterfaces();

            // Register data access types
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterType<WeeeMigrationContext>().AsSelf().SingleInstance();
            builder.RegisterType<WeeeContext>().AsSelf().SingleInstance();
            builder.RegisterType<XmlConverter>().As<IXmlConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeBandCalculator>().As<IProducerChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<WhiteSpaceCollapser>().As<IWhiteSpaceCollapser>().InstancePerLifetimeScope();
            builder.RegisterType<Deserializer>().As<IDeserializer>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationProducerChargeCalculatorDataAccess>().As<IMigrationProducerChargeCalculatorDataAccess>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationProducerChargeBandCalculatorChooser>().As<IMigrationProducerChargeBandCalculatorChooser>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationEnvironmentAgencyProducerChargeBandCalculator>().As<IMigrationEnvironmentAgencyProducerChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationTotalChargeCalculatorDataAccess>().As<IMigrationTotalChargeCalculatorDataAccess>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationEnvironmentAgencyProducerChargeBandCalculator>().As<IMigrationChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationProducerAmendmentChargeCalculator>().As<IMigrationChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationRegisteredProducerDataAccess>().As<IMigrationRegisteredProducerDataAccess>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationFetchProducerCharge>().As<IMigrationFetchProducerCharge>().InstancePerLifetimeScope();
        }
    }
}