namespace EA.Weee.DataAccess
{
    using Autofac;
    using DataAccess;
    using EA.Prsd.Core.Autofac;
    using EA.Prsd.Core.Domain;
    using StoredProcedure;

    public class EntityFrameworkModule : Module
    {
        private readonly EnvironmentResolver environment;

        public EntityFrameworkModule(EnvironmentResolver environment)
        {
            this.environment = environment;
        }

        public EntityFrameworkModule()
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
            builder.RegisterTypeByEnvironment(typeof(WeeeContext), environment);
            builder.RegisterTypeByEnvironment<EvidenceDataAccess, IEvidenceDataAccess>(environment);
            builder.RegisterTypeByEnvironment<GenericDataAccess, IGenericDataAccess>(environment);
            builder.RegisterTypeByEnvironment<WeeeTransactionAdapter, IWeeeTransactionAdapter>(environment);
            builder.RegisterTypeByEnvironment<SystemDataDataAccess, ISystemDataDataAccess>(environment);
            builder.RegisterTypeByEnvironment<ObligationDataAccess, IObligationDataAccess>(environment);

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<RegisteredProducerDataAccess>().As<IRegisteredProducerDataAccess>()
                .InstancePerRequest();

            builder.RegisterTypeByEnvironment<QuarterWindowTemplateDataAccess, IQuarterWindowTemplateDataAccess>(environment);

            builder.RegisterType<ProducerSubmissionDataAccess>().As<IProducerSubmissionDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<OrganisationDataAccess>().As<IOrganisationDataAccess>()
                .InstancePerRequest();

            builder.RegisterTypeByEnvironment<SchemeDataAccess, ISchemeDataAccess>(environment);

            builder.RegisterTypeByEnvironment<StoredProcedures, IStoredProcedures>(environment);
            builder.RegisterTypeByEnvironment<EvidenceStoredProcedures, IEvidenceStoredProcedures>(environment);

            builder.RegisterType<ProducerChargeCalculatorDataAccess>().As<IProducerChargeCalculatorDataAccess>()
                .InstancePerRequest();
        }
    }
}