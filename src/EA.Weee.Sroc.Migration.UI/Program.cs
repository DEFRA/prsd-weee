namespace EA.Weee.Sroc.Migration.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Autofac;
    using Autofac.Core;
    using DataAccess;
    using EA.Weee.RequestHandlers;
    using EA.Weee.Xml;
    using EA.Weee.XmlValidation;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xml.MemberRegistration;

    public static class Program
    {
        private static IContainer Container { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Bootstrap();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(Container.Resolve<IUpdateProducerCharges>()));
        }

        private static void Bootstrap()
        {
            // Create the container as usual.
            //container = new Container()

            // Register your types, for instance:
            //container.Register<IUserRepository, SqlUserRepository>(Lifestyle.Singleton);
            //container.Register<IUserContext, WinFormsUserContext>();
            //container.Register<Form1>();

            // Optionally verify the container.
            //container.Verify();
            var builder = new ContainerBuilder();
            // Register individual components
            //builder.RegisterInstance(new SomeRepository).As<ISomeRepository>();
            builder.RegisterModule(new MigrationRegistrationModule());
            //builder.RegisterModule(new EntityFrameworkModule());
            builder.RegisterType<WeeeContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<UserContext>().As<IUserContext>().InstancePerLifetimeScope();
            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().InstancePerLifetimeScope();
            //builder.RegisterModule(new XmlValidationModule());
            builder.RegisterType<XmlConverter>().As<IXmlConverter>().InstancePerLifetimeScope();
            builder.RegisterType<XMLChargeBandCalculator>().As<IXMLChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeCalculator>().As<IProducerChargeCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeBandCalculator>().As<IProducerChargeBandCalculator>().InstancePerLifetimeScope();
            builder.RegisterType<WhiteSpaceCollapser>().As<IWhiteSpaceCollapser>().InstancePerLifetimeScope();
            builder.RegisterType<Deserializer>().As<IDeserializer>().InstancePerLifetimeScope();
            builder.RegisterType<ProducerChargeCalculatorDataAccess>().As<IProducerChargeCalculatorDataAccess>().InstancePerLifetimeScope();
            //builder.RegisterModule(new XmlModule());
            //builder.RegisterModule(new RequestHandlerModule());
            builder.RegisterType<Form1>();

            Container = builder.Build();
        }
    }
}
