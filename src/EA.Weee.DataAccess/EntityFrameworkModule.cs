namespace EA.Weee.DataAccess
{
    using Autofac;
    using DataAccess;
    using EA.Prsd.Core.Domain;
    using StoredProcedure;

    public class EntityFrameworkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeeeContext>().AsSelf().InstancePerRequest();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<RegisteredProducerDataAccess>().As<IRegisteredProducerDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<QuarterWindowTemplateDataAccess>().As<IQuarterWindowTemplateDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<SystemDataDataAccess>().As<ISystemDataDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<ProducerSubmissionDataAccess>().As<IProducerSubmissionDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<OrganisationDataAccess>().As<IOrganisationDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<SchemeDataAccess>().As<ISchemeDataAccess>()
                .InstancePerRequest();

            builder.RegisterType<StoredProcedures>().As<IStoredProcedures>()
                .InstancePerRequest();
        }
    }
}