namespace EA.Weee.Xml
{
    using Autofac;
    using Converter;
    using Deserialization;
    using EA.Prsd.Core.Autofac;
    using EA.Weee.DataAccess.DataAccess;
    using MemberRegistration;

    public class XmlModule : Module
    {
        private readonly EnvironmentResolver environment;

        public XmlModule(EnvironmentResolver environment)
        {
            this.environment = environment;
        }

        public XmlModule()
        {
            this.environment = new EnvironmentResolver()
            {
                HostEnvironment = HostEnvironmentType.Owin,
                IocApplication = IocApplication.RequestHandler,
                IsTestRun = false
            };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterTypeByEnvironment<ProducerChargeBandCalculator, IProducerChargeBandCalculator>(environment);
            builder.RegisterTypeByEnvironment<WhiteSpaceCollapser, IWhiteSpaceCollapser>(environment);
            builder.RegisterTypeByEnvironment<Deserializer, IDeserializer>(environment);
            builder.RegisterTypeByEnvironment<XmlConverter, IXmlConverter>(environment);
            builder.RegisterTypeByEnvironment<EnvironmentAgencyProducerChargeBandCalculator, IEnvironmentAgencyProducerChargeBandCalculator>(environment);
            builder.RegisterTypeByEnvironment<EnvironmentAgencyProducerChargeBandCalculator, IProducerChargeBandCalculator>(environment);
            builder.RegisterTypeByEnvironment<ProducerAmendmentChargeCalculator, IProducerChargeBandCalculator>(environment);
            builder.RegisterTypeByEnvironment<ProducerChargeBandCalculatorChooser, IProducerChargeBandCalculatorChooser>(environment);
            builder.RegisterTypeByEnvironment<FetchProducerCharge, IFetchProducerCharge>(environment);
        }
    }
}
