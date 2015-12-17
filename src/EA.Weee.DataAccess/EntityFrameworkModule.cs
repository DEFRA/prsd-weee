namespace EA.Weee.DataAccess
{
    using Autofac;
    using EA.Prsd.Core.Domain;
    using Repositories;

    public class EntityFrameworkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeeeContext>().AsSelf().InstancePerRequest();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<RegisteredProducerRepository>().As<IRegisteredProducerRepository>()
                .InstancePerRequest();
        }
    }
}