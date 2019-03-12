namespace EA.Weee.Sroc.Migration.Autofac
{
    using global::Autofac;
    using DataAccess;
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
            builder.RegisterType<XMLChargeBandCalculator>().As<IXMLChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeCalculator>().As<IProducerChargeCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeBandCalculator>().As<IProducerChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<WhiteSpaceCollapser>().As<IWhiteSpaceCollapser>().InstancePerLifetimeScope();
            builder.RegisterType<Deserializer>().As<IDeserializer>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeCalculatorDataAccess>().As<IProducerChargeCalculatorDataAccess>().InstancePerLifetimeScope();
            builder.RegisterType<MigrationProducerChargeCalculatorDataAccess>().As<IProducerChargeCalculatorDataAccess>().InstancePerLifetimeScope();            
        }
    }
}