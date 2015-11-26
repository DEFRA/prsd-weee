namespace EA.Weee.Xml
{
    using Autofac;
    using Converter;
    using Deserialization;

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
        }
    }
}
