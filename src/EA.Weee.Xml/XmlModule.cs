namespace EA.Weee.Xml
{
    using Autofac;
    using Converter;
    using Deserialization;
    using MemberRegistration;

    public class XmlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProducerChargeBandCalculator>()
                .As<IProducerChargeBandCalculator>()
                .InstancePerRequest();

            builder.RegisterType<WhiteSpaceCollapser>()
                .As<IWhiteSpaceCollapser>()
                .InstancePerRequest();

            builder.RegisterType<Deserializer>()
                .As<IDeserializer>();

            builder.RegisterType<XmlConverter>()
                .As<IXmlConverter>()
                .InstancePerRequest();

            builder.RegisterType<EnvironmentAgencyProducerChargeBandCalculator>()
                .As<IEnvironmentAgencyProducerChargeBandCalculator>()
                .InstancePerRequest();

            builder.RegisterType<EnvironmentAgencyProducerChargeBandCalculator>()
                .As<IProducerChargeBandCalculator>()
                .InstancePerRequest();

            builder.RegisterType<ProducerAmendmentChargeCalculator>()
                .As<IProducerChargeBandCalculator>()
                .InstancePerRequest();

            builder.RegisterType<ProducerChargeBandCalculatorChooser>()
                .As<IProducerChargeBandCalculatorChooser>()
                .InstancePerRequest();

            builder.RegisterType<FetchProducerCharge>()
                .As<IFetchProducerCharge>()
                .InstancePerRequest();
        }
    }
}
