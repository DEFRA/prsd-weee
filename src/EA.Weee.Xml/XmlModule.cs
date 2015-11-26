namespace EA.Weee.Xml
{
    using Autofac;
    using MemberRegistration;

    public class XmlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProducerChargeBandCalculator>()
                .As<IProducerChargeBandCalculator>()
                .InstancePerRequest();
        }
    }
}
